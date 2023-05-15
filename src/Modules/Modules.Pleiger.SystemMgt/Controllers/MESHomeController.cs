using InfrastructureCore;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Admin.Services.ServiceImp;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.SystemMgt.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pleiger.SystemMgt.Controllers
{
    public class MESHomeController : BaseController
    {
        IBoardManagementService boardManagementService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IChartService _chartService;


        public MESHomeController(IBoardManagementService boardManagementService, IHttpContextAccessor contextAccessor, IChartService chartService, IAccessMenuService accessMenuService) : base(contextAccessor)
        {
            this.boardManagementService = boardManagementService;
            this.contextAccessor = contextAccessor;
            _chartService = chartService;
            _accessMenuService = accessMenuService;
            //this._partnerService = partnerService;
            //this._mesItemSlipService = mesItemSlipService;
        }


        public IActionResult Index()
        {
            ViewBag.Language = CurrentLanguages;
            if (!CheckSessionIsExists())
            {                
                string url = Url.Action("MESLogin", "MESAccount");
                return Redirect(url);
            }
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
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.EDIT_YN = ListButtonPermissionByUser[0].EDIT_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.UPLOAD_FILE_YN = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
            }
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            return View();            
        }
        public IActionResult ShowNoticeForItemRemark()
        {
            return PartialView("ShowNoticePartner");
        }

        public IActionResult ShowNoticeForItemDeliveryDate()
        {
            return PartialView("ShowNoticePartnerByDateChange");
        }
        

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
         
            return list;
        }
        #endregion
    }
}
