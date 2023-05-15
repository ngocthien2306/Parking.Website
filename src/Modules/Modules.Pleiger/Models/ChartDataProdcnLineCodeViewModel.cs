using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class ChartDataProdcnLineCodeViewModel
    {
        public string ProdcnLineCode { get; set; }
        public string ProductLineName { get; set; }
        public int PlannedQty { get; set; }
        public int ProducedQty { get; set; }
    }
}
