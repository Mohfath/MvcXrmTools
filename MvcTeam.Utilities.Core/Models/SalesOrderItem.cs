using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcTeam.Utilities.Models
{
    public class SalesOrderItem : ItemDetail
    {
        public SalesOrderItem()
        {
            Entity = new Entity("saleorderdetail");
        }
        public SalesOrderItem(Entity item) : base(item)
        {
        }
        public string FieldName { get { return (string)Entity["new_name"]; } }
        public Guid InvoiceId { get { return ((EntityReference)Entity["invoiceid"]).Id; } set { Entity["invoiceid"] = new EntityReference("invoice", value); } }
        public string Name { get { return GetValue("salesorderdetailname"); } set { SetValue("salesorderdetailname", value); } }



    }
}
