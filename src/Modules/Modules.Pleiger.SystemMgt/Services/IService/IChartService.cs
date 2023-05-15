using System;
using System.Collections.Generic;
using System.Text;
using Modules.Pleiger.CommonModels;

namespace Modules.Pleiger.SystemMgt.Services.IService
{
    public interface IChartService
    {
        List<ChartDataViewModel> GetDataChartByMonth();
        List<ChartDataViewModel> GetDataChart12MonthsInThisYear();
        List<ChartDataViewModel> GetProducedQtyPlannedQty4WeeksInMonth();
        List<ChartDataProdcnLineCodeViewModel> GetDataChartProdcnLineCode();
    }
}
