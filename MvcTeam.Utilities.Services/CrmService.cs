using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace MvcTeam.Utilities.Services
{

    public class CrmService
    {
        private IOrganizationService _orgService;
        private ITracingService _tracingService;


        public CrmService()
        {

        }

        public CrmService(IOrganizationService service, ITracingService tracingService)
        {
            _orgService = service;
            _tracingService = tracingService;
        }
        public CrmService(ApplicationConfig config)
        {
            ClientCredentials credentials = new ClientCredentials();
            if (config.UseCurrent)
            {
                credentials.Windows.ClientCredential = new System.Net.NetworkCredential();
            }
            else
            {
                credentials.Windows.ClientCredential = new System.Net.NetworkCredential(config.CrmUsername, config.CrmPassword, config.CrmDomain);
            }

            _orgService = new OrganizationServiceProxy(new Uri(config.CrmServiceUrl), null, credentials, null);
        }

        public SaleOrder GetSaleOrderById(Guid saleOrderId)
        {
            var order = new SaleOrder(_orgService.Retrieve("salesorder", saleOrderId, new ColumnSet(true)));
            order.SaleOrderItems = GetSaleOrderItemsForSaleOrder(saleOrderId);
            return order;
        }

        private List<SalesOrderItem> GetSaleOrderItemsForSaleOrder(Guid saleOrderId)
        {
            var output = new List<SalesOrderItem>();
            QueryExpression query = new QueryExpression
            {
                EntityName = "salesorderdetail",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression { Conditions = { new ConditionExpression { AttributeName = "salesorderid", Operator = ConditionOperator.Equal, Values = { saleOrderId } } } }
            };
            var result = _orgService.RetrieveMultiple(query);
            foreach (var item in result.Entities)
            {
                output.Add(new SalesOrderItem(item));
            }
            return output;
        }

        internal Invoice GetInvoiceById(Guid id)
        {
            var invoice = new Invoice(_orgService.Retrieve("invoice", id, new ColumnSet(true)));
            invoice.InvoiceItems = GetInvoiceItemsForInvoice(id).ToList();
            return invoice;
        }

        private IEnumerable<InvoiceItem> GetInvoiceItemsForInvoice(Guid invoiceId)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = "invoicedetail",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression { Conditions = { new ConditionExpression { AttributeName = "invoiceid", Operator = ConditionOperator.Equal, Values = { invoiceId } } } }
            };
            var result = _orgService.RetrieveMultiple(query);
            foreach (var item in result.Entities)
            {
                yield return new InvoiceItem(item);
            }
        }

        public void DeleteItem(EntityObject item)
        {
            _orgService.Delete(item.Entity.LogicalName, item.Id);
        }

        internal IEnumerable<Invoice> GetInvoicesForSaleOrder(Guid id)
        {
            var query = new QueryExpression("invoice");
            query.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, id);
            query.ColumnSet = new ColumnSet(true);

            foreach (var item in _orgService.RetrieveMultiple(query).Entities)
            {
                var tempInvoice = new Invoice(item);
                tempInvoice.InvoiceItems = GetInvoiceItemsForInvoice(tempInvoice.Id);
                yield return tempInvoice;
            }
        }

        public void UpdateEntity(EntityObject item)
        {
            _orgService.Update(item.Entity);
        }

        internal void ClearInvoiceItems(Guid id)
        {
            var items = GetInvoiceItemsForInvoice(id);

            foreach (var item in items)
            {
                _orgService.Delete("invoicedetail", item.Id);
            }
        }

        internal void AddInvoiceItemsToInvoice(Guid id, IEnumerable<InvoiceItem> computedInvoiceItems)
        {
            foreach (var item in computedInvoiceItems.ToList())
            {
                item.InvoiceId = id;
                _orgService.Create(item.Entity);
            }
        }
    }
}

