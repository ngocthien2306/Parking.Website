using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Services.IService
{
    public interface IChartService
    {
        List<ChartDataViewModel> GetDataChartByMonth();
        List<ChartDataViewModel> GetDataChart12MonthsInThisYear();
        List<ChartDataViewModel> GetProducedQtyPlannedQty4WeeksInMonth();
        List<ChartDataProdcnLineCodeViewModel> GetDataChartProdcnLineCode();
    }
}
