using Microsoft.Xrm.Sdk;
using System;

namespace MvcTeam.Utilities.Models
{

    public class PriceListItem : EntityObject
    {
        public PriceListItem()
        {
            Entity = new Entity("productpricelevel");
        }

        public PriceListItem(Entity entity) : base(entity)
        {

        }

        public Guid ProductId { get { return ((EntityReference)Entity["productid"]).Id; } set { Entity["productid"] = new EntityReference("product", value); } }

        public Money Price { get { return new Money(decimal.Parse(GetValue("amount"))); } set { SetValue("amount", value); } }
        public Guid UomId { get { return ((EntityReference)Entity["uomid"]).Id; } set { Entity["uomid"] = new EntityReference("uom", value); } }
        public string UomIdName { get { return ((EntityReference)Entity["uomid"]).Name; } }

        public Guid PriceListId
        {
            get { return ((EntityReference)Entity["pricelevelid"]).Id; }
            set { SetValue("pricelevelid", new EntityReference("pricelevel", value)); }
        }

        
    }
}