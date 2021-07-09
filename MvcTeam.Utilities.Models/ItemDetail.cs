using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.Models
{
    public class ItemDetail : EntityObject
    {
        public ItemDetail()
        {

        }
        public ItemDetail(Entity entity) : base(entity)
        {

        }
        public int ItemNo { get; set; }
        public decimal ProfitPercent { get { return GetValue("mvc_profitpercent") == "" ? 0 : decimal.Parse(GetValue("mvc_profitpercent")); } set { SetValue("mvc_profitpercent", value); } }
        public string Brand { get { return GetValue("mvc_vendorname"); } set { SetValue("mvc_vendorname", value); } }
        public string PartNumber { get { return (string)Entity["mvc_vendorpartnumber"]; } set { SetValue("mvc_vendorpartnumber", value); } }
        public string RequestedBrand { get { return (string)Entity["mvc_vendornamerequested"]; } set { SetValue("mvc_vendornamerequested", value); } }
        public string RequestedPartNumber { get { return (string)Entity["mvc_vendorpartnumberrequested"]; } set { SetValue("mvc_vendorpartnumberrequested", value); } }
        public string BasicUomName
        {
            get { return (string)Entity["mvc_uomname"]; }
            set { SetValue("mvc_uomname", value); }
        }

        /// <summary>
        /// استفاده در محصولات نوشتنی برای لینک شدن به محصول اصلی
        /// برای راحتی در مشاهده سابقه قیمت ها
        /// </summary>
        public Guid ProductLinkId { get { return ((EntityReference)Entity["mvc_productlinkid"]).Id; } set { Entity["mvc_productlinkid"] = new EntityReference("product", value); } }


        public string DescriptionEn { get { return GetValue("mvc_descriptionenglish"); } set { SetValue("mvc_descriptionenglish", value); } }
        public decimal TaxPercent { get { return GetValue("mvc_taxpercent") == "" ? 0 : decimal.Parse(GetValue("mvc_taxpercent")); } set { Entity["mvc_taxpercent"] = value; } }
        public decimal StockQuantity { get { return (decimal)Entity["mvc_supplierstockquantity"]; } set { Entity["mvc_supplierstockquantity"] = value; } }
        public decimal SupplierPricePerUnit { get { return GetValue("mvc_supplierunitprice") == "" ? 0 : decimal.Parse(GetValue("mvc_supplierunitprice")); } set { SetValue("mvc_supplierunitprice", value); } }
        public Guid CurrencyUnit
        {
            set { Entity["transactioncurrencyid"] = new EntityReference("currrency", value); }
        }

        /// <summary>
        /// این عدد از این ارز به ریال است مثلا ۱۵۰۰۰ تومان
        /// </summary>
        public decimal ExchangePrice { get { return GetValue("mvc_exchangevalue") == "" ? 0 : decimal.Parse(GetValue("mvc_exchangevalue")); } set { Entity["mvc_exchangevalue"] = value; } }

        public Guid BasicUomId { get; set; }
        public decimal SupplierRialPrice { get { return SupplierPricePerUnit * ExchangePrice; } }

        public DateTime LastSupplierPriceDate
        {
            get { return (DateTime)Entity["mvc_lastsupplierpricedate"]; }
            set { Entity["mvc_lastsupplierpricedate"] = value; }
        }
        public decimal LastPriceDifference { get { return (decimal)Entity["mvc_lastpricedifference"]; } set { Entity["mvc_lastpricedifference"] = value; } }
        public decimal LastSupplierPrice { get { return (decimal)Entity["mvc_lastsupplierprice"]; } set { Entity["mvc_lastsupplierprice"] = value; } }
        public decimal SupplierTotalPrice { get { return GetValue("mvc_suppliertotalprice") == "" ? 0 : decimal.Parse(GetValue("mvc_suppliertotalprice")); } set { SetValue("mvc_suppliertotalprice", value); } }

        public decimal CurrencyPricePerUnit
        {
            get { return GetValue("mvc_currencypriceperunit") == "" ? 0 : decimal.Parse(GetValue("mvc_currencypriceperunit")); }
            set { SetValue("mvc_currencypriceperunit", value); }
        }

        public decimal TotalCurrencyPricePerUnit
        {
            get { return GetValue("mvc_totalcurrencyprice") == "" ? 0 : decimal.Parse(GetValue("mvc_totalcurrencyprice")); }
            set { SetValue("mvc_totalcurrencyprice", value); }
        }

        public Guid UomId { get { return ((EntityReference)Entity["uomid"]).Id; } set { Entity["uomid"] = new EntityReference("uom", value); } }
        public string Description { get { return GetValue("productdescription"); } set { SetValue("productdescription", value); SetValue("description", value); } }
        public bool IsProductOverriden { get { return bool.Parse(GetValue("isproductoverridden")); } set { SetValue("isproductoverridden", value); } }
        public Money PricePerUnit { get { return new Money(decimal.Parse(GetValue("priceperunit"))); } set { SetValue("priceperunit", value); } }
        public decimal Quantity { get { return (decimal)Entity["quantity"]; } set { Entity["quantity"] = value; } }
        public Money Discount { get { return new Money(decimal.Parse(string.IsNullOrEmpty(GetValue("manualdiscountamount")) ? "0" : GetValue("manualdiscountamount"))); } set { SetValue("manualdiscountamount", value); } }
        public Money Tax { get { return new Money(decimal.Parse(string.IsNullOrEmpty(GetValue("tax")) ? "0" : GetValue("tax"))); } set { SetValue("tax", value); } }
        public Guid ProductId { get { return Entity.Contains("productid") ? ((EntityReference)Entity["productid"]).Id : Guid.Empty; } set { Entity["productid"] = new EntityReference("product", value); } }


    }

}
