using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESProductDelivery
    {
        public int No { get; set; }
        public string CustWH { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverQty { get; set; }
    }
}
