using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class ChartDataViewModel
    {
        public string CreateAt { get; set; }
        public int DayOfMonth { get; set; }
        public int MonthOfYear { get; set; }
        public int CurrentYear { get; set; }
        public int ProductedQty { get; set; }
        public int PlannedQty { get; set; }
        public int DonedQtyWeek1 { get; set; }
        public int DonedQtyWeek2 { get; set; }
        public int DonedQtyWeek3 { get; set; }
        public int DonedQtyWeek4 { get; set; }
        public int Week { get; set; }
        public int DonedQtyWeek { get; set; } //dai dien cho cai donedqty
    }
}
