using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace MvcTeam.Utilities.Models
{
    public class Invoice : EntityObject
    {
        public Invoice(Entity entity) : base(entity)
        {
        }

        public Guid SaleOrderId
        { get { return ((EntityReference)Entity["salesorderid"]).Id; } set { Entity["salesorderid"] = new EntityReference("salesorder", value); } }
        public IEnumerable<InvoiceItem> InvoiceItems { get; set; }
    }
}
