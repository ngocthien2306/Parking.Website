using DocumentFormat.OpenXml.Office2010.ExcelAc;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pleiger.Controllers
{
    public class MESHomeController : BaseController
    {
        IBoardManagementService boardManagementService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IChartService _chartService;

        public MESHomeController(IBoardManagementService boardManagementService, IHttpContextAccessor contextAccessor, IChartService chartService) : base(contextAccessor)
        {
            this.boardManagementService = boardManagementService;
            this.contextAccessor = contextAccessor;
            _chartService = chartService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            if (!CheckSessionIsExists())
            {
                string url = Url.Action("MESLogin", "MESAccount");
                return Redirect(url);
            }
            // Quan add 2021-01-15
            // 
            var listMenuAuthorized = CurrentUser.AuthorizedMenus;
            var listMenuNotice = new List<SYMenu>();
            if (listMenuAuthorized.Count > 0)
            {
                listMenuNotice = listMenuAuthorized.Where(x => x.MenuPath.Contains("/CB?bid=") && x.MenuLevel == 2 && x.SiteID == CurrentUser.SiteID).ToList();
            }
            var result = boardManagementService.GetListBoardContentConfirm(CurrentUser.UserID, listMenuNotice);

            return View(result);
        }

        // Dash Board
        public IActionResult DashBoard()
        {
            return View();
        }

        #endregion

        #region "Insert - Update - Delete"



        #endregion
        #region Render Chart
        public object GetDataChartByMonth()
        {
            List<ChartDataViewModel> list = _chartService.GetDataChartByMonth();
            return list;
        }
        public object GetDataChartProdcnLineCode()
        {
            List<ChartDataProdcnLineCodeViewModel> list = _chartService.GetDataChartProdcnLineCode();
            return list;
        }
        public object GetDataChart12MonthsInThisYear()
        {
            List<ChartDataViewModel> list = _chartService.GetDataChart12MonthsInThisYear();
            return list;
        }
        public object GetDataChart4WeeksCurrentMonth()
        {
               List<ChartDataViewModel> list = _chartService.GetProducedQtyPlannedQty4WeeksInMonth();
           // List<ChartDataViewModel> list = new List<ChartDataViewModel>();
            //list.Add(new ChartDataViewModel { wee });
            return list;
        }
        #endregion
    }
}
