using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace MvcTeam.Utilities.Models
{
    public class SaleOrder : EntityObject
    {
        public SaleOrder(Entity entity) : base(entity)
        {
        }

        public IEnumerable<SalesOrderItem> SaleOrderItems { get; set; }
    }
}
