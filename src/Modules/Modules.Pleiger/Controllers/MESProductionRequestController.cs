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

namespace Modules.Pleiger.Controllers
{
    public class MESProductionRequestController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESComCodeService mESComCodeService;
        private readonly IMESProductionRequestService mESProductionRequestService;
        private readonly IMESItemService mESItemService;
        private readonly IMESItemPartnerService mESItemPartnerService;

        public MESProductionRequestController(IHttpContextAccessor contextAccessor, IMESComCodeService mESComCodeService, IMESProductionRequestService mESProductionRequestService
            , IMESItemService mESItemService, IMESItemPartnerService mESItemPartnerService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.mESComCodeService = mESComCodeService;
            this.mESProductionRequestService = mESProductionRequestService;
            this.mESItemService = mESItemService;
            this.mESItemPartnerService = mESItemPartnerService;
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
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;

            return View();
        }

        // Search list Production Request
        [HttpGet]
        public IActionResult SearchListProductionRequest(string projectCode, 
            string userProjectCode ,string requestType, 
            string customerName,  string itemCode,string itemName,
            string requestStartDate,string requestEndDate,string projectStatus,string UserCode)
        {
            var data = mESProductionRequestService.SearchListProductionRequest(projectCode, userProjectCode, requestType, customerName, itemCode, itemName
                , requestStartDate,requestEndDate, projectStatus, UserCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Show detail Production Request
        [HttpGet]
        public IActionResult ShowDetailProductionRequest(string projectCode)
        {
            string url = "/MESProductionRequest/ShowDetailProductionRequest?projectCode=" + projectCode;
            ViewBag.Url = url;

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

            return PartialView("DetailProductionRequest", model);
        }

        // Get list Sale Project
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

        // Get list Material
        [HttpGet]
        public IActionResult GetListMaterial()
        {
            var data = mESItemService.GetListData();
            return Json(data);
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

        // Check Stk Qty is enough or not for Request Production
        [HttpGet]
        public IActionResult CheckStkQtyIsEnough(string projectCode)
        {
            var data = mESProductionRequestService.CheckEnoughItemQty(projectCode);
            return Json(data);
        }

        #endregion

        #region "Insert - Update - Delete"

        // Save data Production Request
        [HttpPost]
        public IActionResult SaveDataProductionRequest(string projectCode, string requestCode, string requestType, string userIDRequest, string requestDate, string requestMessage, string listItemRequest, string listItemPO)
        {
            var model = new MES_SaleProject();
            model.ProjectCode = projectCode;
            model.RequestCode = string.IsNullOrEmpty(requestCode) ? "PQ" + projectCode.Substring(2, projectCode.Length - 2) : requestCode;
            model.UserIDRequest = string.IsNullOrEmpty(userIDRequest) ? CurrentUser.UserID : userIDRequest;
            model.RequestType = requestType;
            model.RequestDate = requestDate;
            model.RequestMessage = requestMessage;

            var result = mESProductionRequestService.SaveDataProductionRequest(model, listItemRequest, listItemPO);
            return Json(result);
        }

        // Send Production Request
        [HttpPost]
        public IActionResult SendProductionRequest(string projectCode)
        {
            var result = mESProductionRequestService.SendRequestProduction(projectCode);

            return Json(result);
        }

        #endregion
    }
}
