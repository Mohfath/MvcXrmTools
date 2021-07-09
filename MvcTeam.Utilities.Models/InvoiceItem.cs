using System;
using Microsoft.Xrm.Sdk;

namespace MvcTeam.Utilities.Models
{
    public class InvoiceItem : ItemDetail
    {
        public InvoiceItem()
        {
            Entity = new Entity("invoicedetail");
        }
        public InvoiceItem(Entity item) : base(item)
        {
        }
        public string FieldName { get { return (string)Entity["new_name"]; } }
        public Guid InvoiceId { get { return ((EntityReference)Entity["invoiceid"]).Id; } set { Entity["invoiceid"] = new EntityReference("invoice", value); } }
        public string Name { get { return GetValue("invoicedetailname"); } set { SetValue("invoicedetailname", value); } }



    }
}