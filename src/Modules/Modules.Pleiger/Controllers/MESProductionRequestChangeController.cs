using System.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Modules.Admin.Services.IService;

namespace Modules.Pleiger.Controllers
{
    public class MESProductionRequestChangeController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESComCodeService mESComCodeService;
        private readonly IMESProductionRequestService mESProductionRequestService;
        private readonly IMESItemService mESItemService;
        private readonly IMESItemPartnerService mESItemPartnerService;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserService _userService;
        private readonly IMESBOMMgtService _mESBOMMgtService;

        public MESProductionRequestChangeController(IHttpContextAccessor contextAccessor, IMESComCodeService mESComCodeService, IMESProductionRequestService mESProductionRequestService
            , IMESItemService mESItemService, IMESItemPartnerService mESItemPartnerService, IPurchaseService purchaseService
            , IMESBOMMgtService mESBOMMgtService, IUserService userService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.mESComCodeService = mESComCodeService;
            this.mESProductionRequestService = mESProductionRequestService;
            this.mESItemService = mESItemService;
            this.mESItemPartnerService = mESItemPartnerService;
            this._purchaseService = purchaseService;
            this._userService = userService;
            this._mESBOMMgtService = mESBOMMgtService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.UserCode = CurrentUser.UserCode;
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;
            return View();
        }

        // Search list Production Request
        [HttpGet]
        public IActionResult SearchListProductionRequest(string projectCode,
            string userProjectCode, string requestType,
            string customerName, string itemCode,string itemName,
            string requestStartDate, string requestEndDate, string projectStatus,string UserCode)
        {
            var data = mESProductionRequestService.SearchListProductionRequest(projectCode, userProjectCode, requestType, customerName, itemCode, itemName
                , requestStartDate, requestEndDate, projectStatus, UserCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Show detail Production Request
        [HttpGet]
        public IActionResult ShowDetailProductionRequest(string projectCode)
        {
            //string url = "/MESProductionRequest/ShowDetailProductionRequestChange?projectCode=" + projectCode;
            //ViewBag.Url = url;

            var model = mESProductionRequestService.GetDataDetail(projectCode);

            // Get list Item Request
            var listItemRequest = mESProductionRequestService.GetListItemRequest(projectCode);
            model.ListItemRequest = listItemRequest;

            // Get list Item PO
            var listItemPO = mESProductionRequestService.GetListItemPO(projectCode);
            model.ListItemPO = listItemPO;

            var listArrivalRequestDate = model.ListItemPO.Where(x => x.StatusCode != "ORST03").Select(x => new POArrivalRequestDate
            {
                PartnerCode = x.PartnerCode,
                PONumber = x.PONumber,
                ArrivalRequestDate = x.ArrivalRequestDate
            }).Distinct().ToList();
            ViewBag.ListArrivalRequestDate = listArrivalRequestDate;

            return PartialView("DetailProductionRequestChange", model);
        }
        // Quan add 2020/09/11
        // Get list Item Request
        [HttpGet]
        public IActionResult ShowDetailtemRequest(string projectCode, int menuID, string vbParent, string InitialCode)
        {
            var model = mESProductionRequestService.GetDataDetail(projectCode);
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.InitialCode = InitialCode;
            ViewBag.ProjectName = model.ProjectName;
            ViewBag.ProjectCode = model.ProjectCode;
            ViewBag.UserProjectCode = model.UserProjectCode;
            ViewBag.requestCode = model.RequestCode;
            ViewBag.PartnerName = model.PartnerName;
            ViewBag.ProjectStatus = model.ProjectStatus;
            ViewBag.ProjectStatusName = model.ProjectStatusName;
            ViewBag.RequestDate = model.RequestDate;
            ViewBag.RequestDate = model.RequestDate;
            ViewBag.OrderQuantity = model.OrderQuantity;
            ViewBag.RequestMessage = model.RequestMessage;
            ViewBag.ItemName = model.ItemName;
            ViewBag.ItemCode = model.ItemCode;
            ViewBag.RequestType = model.RequestType;
            ViewBag.UserRequest = CurrentUser.UserName;
            ViewBag.MenuID = menuID;
            ViewBag.Parent = vbParent;
            return PartialView("DetailProductionRequestChange", model);
        }
        public IActionResult ReloadRequestMst(string projectCode, int menuID)
        {
            var model = mESProductionRequestService.GetDataDetail(projectCode);
            return Content(JsonConvert.SerializeObject(model), "application/json");
        }
        // Quan add 2020/09/11
        // Get list Item Request in grid
        [HttpGet]
        // Get list Material
        [HttpGet]
        public IActionResult GetListItemMaterialNotexist(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var data = mESItemService.GetListItemMaterialNotexist(projectCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Quan add 2020/09/11
        // Get list Item Request
        [HttpGet]
        public IActionResult GetlistItemRequest(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var listItemRequest = mESProductionRequestService.GetListItemRequest(projectCode);
            var loadResult = DataSourceLoader.Load(listItemRequest, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        public IActionResult CheckItemIxits(string ItemCode, List<MES_Item> listData)
        {

            var result = listData.Select(x => x.ItemCode = ItemCode);

            return Json(result);
        }
        // Quan add 2020/09/14
        // Check Request Production
        // ReqQty < StkQty
        [HttpGet]
        public IActionResult CheckStkQtyIsEnough(string projectCode,int orderQuantity)
        {
           
            var listItemRequest = mESProductionRequestService.GetListItemRequest(projectCode);

            if( listItemRequest.Count == 0)
            {
                return Json("1");
            }
            else
            {
                var ResultCheck = listItemRequest.Where(x => x.ReqQty * orderQuantity > x.RealQty).FirstOrDefault();
                var ResultCheckRealQty = listItemRequest.Where(x => x.ReqQty == 0 || x.RealQty == null).FirstOrDefault();
                if (ResultCheck != null || ResultCheckRealQty != null)
                {
                    return Json("1");
                }
                else
                {
                    return Json(null);
                }
            }
        }
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = mESProductionRequestService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list Common Code by groupCode
        [HttpGet]
        public IActionResult GetListCommonCode(string groupCode)
        {
            var data = mESComCodeService.GetListComCodeDTL(groupCode);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // Quan add
        // Get list Common Code Category IMTP01, IMTP02
        [HttpGet]
        public IActionResult GetListComCodeCategoryMaterial(string groupCD,string baseCD1, string baseCD2, string baseCD3, string baseCD4)
        {
            var data = mESComCodeService.GetListComCodeCategoryMaterial(groupCD, baseCD1, baseCD2, baseCD3, baseCD4);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // Get list Material
        [HttpGet]
        public IActionResult GetListMaterial(DataSourceLoadOptions loadOptions)
        {
            //int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
            //int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

            //var data = mESItemService.GetListData(pageSize, itemSkip);
            var data = mESItemService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list Material by ItemClassCode
        [HttpGet]
        public IActionResult GetListMaterialByItemClassCode(DataSourceLoadOptions loadOptions, string ItemClassCode)
        {
            var data = mESItemService.GetListMaterialByItemClassCode(ItemClassCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get Material detail
        [HttpGet]
        public IActionResult GetDetailMaterial(string itemCode)
        {
            var data = mESItemService.GetDetail(itemCode);
            return Json(data);
        }

        // Get list Partner by itemCode
        [HttpGet]
        public IActionResult GetListPartnerByItemCode(string itemCode)
        {
            var data = mESItemPartnerService.GetListPartnerByItem(itemCode);
            return Json(data);
        }

        // Get item detail of Partner
        [HttpGet]
        public IActionResult GetDetailItemOfPartner(string itemCode, string partnerCode)
        {
            var data = mESItemPartnerService.GetItemDetail(itemCode, partnerCode);
            return Json(data);
        }

        #endregion

        #region "Insert - Update - Delete"

        // Save data Production Request
        // Save data list Itemquest
        [HttpPost]
        public IActionResult SaveDataProductionRequestChange(string projectCode, string requestCode, string requestType, string userIDRequest, string requestDate, string requestMessage, List<ItemRequest> listItemRequest)
        {
            //listItemRequest.RemoveAt(0);          
            string RequestCode = string.IsNullOrEmpty(requestCode) ? "PQ" + projectCode.Substring(2, projectCode.Length - 2) : requestCode;

            var result = mESProductionRequestService.SaveDataProductionRequestChange(projectCode, RequestCode, requestType, CurrentUser.UserID, requestDate, requestMessage, listItemRequest);
            return Json(result);
        }
        public IActionResult UpdateDataProductionRequestChange(string projectCode, string requestCode, string requestType, string userIDRequest, string requestDate, string requestMessage)
        {

            var model = new MES_SaleProject();
            string RequestCode = string.IsNullOrEmpty(requestCode) ? "PQ" + projectCode.Substring(2, projectCode.Length - 2) : requestCode;

            var result = mESProductionRequestService.UpdateDataProductionRequestChange(projectCode, RequestCode, requestType, CurrentUser.UserID, requestDate, requestMessage);
            return Json(result);
        }
        // Quan add 2020/09/14
        // Send Production Request
        [HttpPost]
        public IActionResult RequestProduction(string projectCode)
        {
            var result = mESProductionRequestService.RequestProduction(projectCode, CurrentUser.UserID);
            return Json(result);
        }
        [HttpPost]
        public IActionResult DeleteGridItemPartList(string ItemCode,string RequestCode)
        {
            var result = mESProductionRequestService.DeleteGridItemPartList(ItemCode, RequestCode);
            return Json(result);
        }
        
        // Quan add 2020/09/16
        // GetI tem ByProject Code In Status ('PJST03','PJST04','PJST05','PJST06')
        [HttpGet]
        public IActionResult GetItemByProjectCodeInStatus(DataSourceLoadOptions loadOptions)
        {
            var data = mESProductionRequestService.GetItemByProjectCodeInStatus();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        /// <summary>
        /// 2020-10-05
        /// Add button "Recall", change state from "Production planning" => to "Production Req".
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RecallProductionRequest(string projectCode)
        {
            var result = mESProductionRequestService.RecallProductionRequest(projectCode, CurrentUser.UserID);
            return Json(result);
        }
        #endregion

        #region Show Popup Item Related To Project

        [HttpGet]
        public IActionResult ProductionRequestPopup(string projectCode, string vbThread)
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();

            return PartialView("ProductionRequestPopup", model);
        }

        [HttpGet]
        public IActionResult GetBOMData(string projectCode)
        {
           // var result = _mESBOMMgtService.GetBOMItemBySalePJCode(projectCode);
           // return Json(result);
            return Json(new object { });
        }

        [HttpGet]
        public IActionResult ProductionRequestAddPartListPopup(string projectCode, string vbThread)
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();

            return PartialView("ProductionRequestAddPartListPopup", model);
        }

        [HttpGet]
        public IActionResult GetCategory(DataSourceLoadOptions loadOptions)
        {
            var data = mESProductionRequestService.GetListCategories();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemClass(DataSourceLoadOptions loadOptions, string categoryCode)
        {
            var data = mESProductionRequestService.GetListItemClass(categoryCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfProject(string ProjectCode, string Category, string ItemClassCode, string ItemCode)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, ItemCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfItemClasscode(string Category, string ItemClassCode, string ItemCode, string ItemName)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = mESProductionRequestService.GetListItemOfItemClassCode(Category, ItemClassCode, ItemCode, ItemName);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        #endregion
    }

}
