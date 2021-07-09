using Microsoft.Xrm.Sdk;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTeam.Utilities.Services
{
    public class App_Invoice_Left_Stock
    {
        private IOrganizationService _organizationService;
        private ITracingService _tracingService;
        private CrmService _crmService;
        public App_Invoice_Left_Stock(IOrganizationService service, ITracingService tracingService)
        {
            _organizationService = service;
            this._tracingService = tracingService;
            _crmService = new CrmService(_organizationService, _tracingService);
        }

        public void StartFunction(Guid id)
        {
            Invoice invoice;
            SaleOrder regardingSalesOrder;
            List<Invoice> invoicesForSaleOrder;
            List<InvoiceItem> oldInvoiceItems;
            List<InvoiceItem> computedInvoiceItems;

            _tracingService.Trace("Got CUstomerID:" + id.ToString());

            //این گردش کار قرار است به ازای ساخت یک رسید فعال شود
            try
            {
                invoice = _crmService.GetInvoiceById(id);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("خطا در دریافت رسید" + ex.Message);
            }

            //شیئ فروش متصل به خود را پیدا میکند
            try
            {
                regardingSalesOrder = _crmService.GetSaleOrderById(invoice.SaleOrderId);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("خطا در دریافت فروش متناظر" + ex.Message);
            }
            //سپس سایر رسیدهای پرداخت متصل به آن را نیز پیدا میکند
            try
            {
                invoicesForSaleOrder = _crmService.GetInvoicesForSaleOrder(regardingSalesOrder.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("خطا در دریافت رسیدهای پرداخت" + ex.Message);
            }
            //در ادامه موجودی های تک تک آن رسید ها را دریافت میکند
            oldInvoiceItems = invoicesForSaleOrder.Where(x => x.Id != invoice.Id).SelectMany(x => x.InvoiceItems).ToList();
            //موجودی رسیدهای قبلی را با موجودی موجودیت فروش مقایسه میکند.
            try
            {
                computedInvoiceItems = ComputeNewList(regardingSalesOrder.SaleOrderItems.ToList(), oldInvoiceItems, invoice.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("خطا در محاسبه موارد باقیمانده" + ex.Message);
            }

            //حال این موارد را چک میکنیم

            try
            {
                foreach (var item in invoice.InvoiceItems)
                {

                    //اگر معادل آنها وجود نداشت حذف میکنیم
                    var regardingComputedItem = computedInvoiceItems.FirstOrDefault(x => x.ProductId == item.ProductId || x.Description == item.Description);

                    if (regardingComputedItem == null)
                    {
                        _crmService.DeleteItem(item);
                        continue;
                    }

                    //اگر وجود داشتند تنها به روز میکنیم
                    var tempInvoiceItem = new InvoiceItem();
                    tempInvoiceItem.Id = item.Id;
                    tempInvoiceItem.Quantity = regardingComputedItem.Quantity;

                    _crmService.UpdateEntity(tempInvoiceItem);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("خطا در چسباندن موارد به این رسید" + ex.Message);
            }

            _tracingService.Trace("App_MvcTeam.Utilities Workflow finished");

        }



        /// <summary>
        /// محاسبه لیست اقلام جدید که باید در این رسید بیایند
        /// </summary>
        /// <param name="invoiceItems"></param>
        /// <param name="oldInvoiceItems"></param>
        /// <returns></returns>
        internal IEnumerable<InvoiceItem> ComputeNewList(IEnumerable<SalesOrderItem> saleOrderItems, IEnumerable<InvoiceItem> previousInvoiceItems, Guid invoiceId)
        {
            //ابتدا لیست محصولات موجود در سیستم را استخراج میکنیم
            var productIds = saleOrderItems.Where(x => !x.IsProductOverriden && x.ProductId != Guid.Empty).Select(x => x.ProductId).Distinct().ToList();
            var productUoms = saleOrderItems.Where(x => !x.IsProductOverriden).Select(x => x.UomId).Distinct().ToList();

            // محصولات writein
            var saleWriteIns = saleOrderItems.Where(x => x.IsProductOverriden);
            var invoiceWriteIns = previousInvoiceItems.Where(x => x.IsProductOverriden);

            if (saleWriteIns.Select(x => x.Name).Count() != saleWriteIns.Select(x => x.Name).Distinct().Count())
            {
                throw new InvalidPluginExecutionException("دو محصول نوشتاری با نام یکسان دارید");
            }

            //به ازای هر ردیف محصول
            //تعداد تحویل شده را محاسبه میکنیم
            foreach (var productId in productIds)
            {
                foreach (var uomId in productUoms)
                {

                    //با کم کردن از تعداد اولیه به تعداد منتظر تحویل میرسیم
                    var deliveredAmount = previousInvoiceItems.Where(x => !x.IsProductOverriden && x.ProductId == productId && x.UomId == uomId).Sum(x => x.Quantity);
                    var newAmount = saleOrderItems.Where(x => !x.IsProductOverriden && x.ProductId == productId && x.UomId == uomId).Sum(x => x.Quantity) - deliveredAmount;

                    var data = new InvoiceItem()
                    {
                        Name = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        Quantity = newAmount,
                        Description = Guid.NewGuid().ToString()
                    };
                    if (data.Quantity > 0)
                    {
                        yield return data;
                    }
                }
            }



            foreach (var item in saleWriteIns)
            {
                //با کم کردن از تعداد اولیه به تعداد منتظر تحویل میرسیم
                var deliveredAmount = previousInvoiceItems.Where(x => x.Description == item.Description).Sum(x => x.Quantity);
                var newAmount = item.Quantity - deliveredAmount;

                var data = new InvoiceItem()
                {
                    Id = item.Id,
                    Quantity = newAmount,
                    Description = item.Description,
                    Name = item.Name,
                    ProductId = Guid.NewGuid()
                };
                if (data.Quantity > 0)
                {
                    yield return data;
                }
            }

        }

    }
}
