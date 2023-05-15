using System.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System.Collections.Generic;
using Modules.Pleiger.CommonModels;
using System;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.FileUpload.Services.IService;
using Modules.Pleiger.SalesProject.Services.IService;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESProductionRequestChangeController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESProductionRequestService _mesProductionRequestService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mesItemService;
        private readonly IMESItemPartnerService _mesItemPartnerService;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserService _userService;
        private readonly IMESBOMMgtService _mesBOMMgtService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IFileService _filesService;


        public MESProductionRequestChangeController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IMESComCodeService mesComCodeService, IMESProductionRequestService mesProductionRequestService
            , IMESItemService mesItemService, IMESItemPartnerService mesItemPartnerService, IPurchaseService purchaseService,
            IUserService userService, IMESBOMMgtService mesBOMMgtService, IFileService filesService, IMESSaleProjectService mesSaleProjectService) : base(contextAccessor)
        {
            this._contextAccessor = contextAccessor;
            this._mesComCodeService = mesComCodeService;
            this._mesProductionRequestService = mesProductionRequestService;
            this._mesItemService = mesItemService;
            this._mesItemPartnerService = mesItemPartnerService;
            this._purchaseService = purchaseService;
            this._userService = userService;
            this._mesBOMMgtService = mesBOMMgtService;
            this._accessMenuService = accessMenuService;
            this._filesService = filesService;
            this._mesSaleProjectService = mesSaleProjectService;

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
        public IActionResult SearchListProductionRequest(MES_SaleProject saleProject, string UserCode, string checkCode,string StartDate,string EndDate)
        {
            var data = _mesProductionRequestService.SearchListProductionRequest(saleProject, UserCode, checkCode, StartDate, EndDate);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        [HttpGet]
        public object GetComboBoxProjectName(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetComboboxProjectName();
            return DataSourceLoader.Load(data, loadOptions);
        }

        // Show detail Production Request
        [HttpGet]
        public IActionResult ShowDetailProductionRequest(string projectCode)
        {
            //string url = "/MESProductionRequest/ShowDetailProductionRequestChange?projectCode=" + projectCode;
            //ViewBag.Url = url;

            var model = _mesProductionRequestService.GetDataDetail(projectCode);

            // Get list Item Request
            var listItemRequest = _mesProductionRequestService.GetListItemRequest(projectCode);
            model.ListItemRequest = listItemRequest;

            // Get list Item PO
            var listItemPO = _mesProductionRequestService.GetListItemPO(projectCode);
            model.ListItemPO = listItemPO;

            var listArrivalRequestDate = model.ListItemPO.Where(x => x.StatusCode != "ORST03").Select(x => new POArrivalRequestDate
            {
                PartnerCode = x.PartnerCode,
                PONumber = x.PONumber,
                ArrivalRequestDate = x.ArrivalRequestDate
            }).Distinct().ToList();
            ViewBag.ListArrivalRequestDate = listArrivalRequestDate;
            ViewBag.FileID = model.FileID;

            return PartialView("DetailProductionRequestChange", model);
        }
        // Quan add 2020/09/11
        // Get list Item Request
        [HttpGet]
        public IActionResult ShowDetailtemRequest(string projectCode, int menuID, string vbParent) 
        {
            var model = new MES_SaleProject();
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            model = _mesSaleProjectService.GetDataDetail(projectCode);
            var listUserPermissionInGroup = _accessMenuService.SelectUserPermissionInGroup(menuID, CurrentUser.UserID);
            var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(menuID, CurrentUser.UserID);
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuID, CurrentUser.UserCode);

            if (listUserPermissionInGroup.Count > 0) // check user Permission in Group
            {
                if (listUserPermissionInGroup[0].SAVE_YN_SUM > 0)
                {
                    model.Btn_Save = true;
                }

            }
            if(listUserPermissionAccessMenu.Count > 0) //check user Permission
            {
                if(listUserPermissionAccessMenu[0].SAVE_YN == true)
                {
                    model.Btn_Save = true;
                }
            }
            string check = "";
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            if(model.InitialCode == true)
            {
                check = "Yes";
            }
            else
            {
                check = "No";
            }

            if (projectCode != null)
            {
                var files = _filesService.GetSYFileUploadMasterByID(model.FileID);
                files.Reverse();
                int count = files.Count;
                model.FileDetail = new List<SYFileUpload>();
                for (int i = 0; i < count; i++)
                {

                    var fileDetail = _filesService.GetSYFileUploadByID(files[i].FileGuid);

                    files[i].FileDetail = fileDetail;
                    fileDetail.FilePathShowBrowser = files[i].FilePath + "\\" + fileDetail.FileNameSave;
                    if (files[count - 1].FileDetail != null)
                    {
                        files[count - 1].FileDetail = fileDetail;
                        files[count - 1].FileDetail.No = 1;
                    }
                    else
                    {
                        files[i].FileDetail = fileDetail;
                        files[i].FileDetail.No = 0;
                    }

                    model.FileDetail.Add(fileDetail);
                }
                ViewBag.SiteID = CurrentUser.SiteID;
                ViewBag.SystemUserType = checkUserLogin.SystemUserType;
                //model.No = 1;
                model.ID = "FileID" + ViewBag.Thread;
                model.UrlPath = "";
                model.Pag_ID = projectCode;
                model.FileMasterID = model.FileID;
                model.Pag_Name = "";
                model.Form_Name = " Drawing Upload File";
                model.Sp_Name = "UPLOAD_FILE_DRAWING";
                if (model.FileID == null || model.FileID == "")
                {
                    model.FileMasterID = Guid.NewGuid().ToString();
                    model.FileID = model.FileMasterID;
                }
                // check user In Group Permission
                if (ListButtonPermissionByUser.Count > 0)
                {
                    model.Upload_File = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
                    model.Delele_File = ListButtonPermissionByUser[0].DELETE_FILE_YN;

                }
            }

            ViewBag.code = check;
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
            ViewBag.ProductType = model.ProductType;

            return PartialView("DetailProductionRequestChange", model);
        }
        [HttpGet]
        // Quan add 2020-11-26
        // Get Project Status 
        // Production Planning check Stark Work
        // Production Request check ReCall
        public IActionResult GetProjectStatus(string projectCode, int menuID, string vbParent)
        {
            var model = _mesProductionRequestService.GetDataDetail(projectCode);
            //var listUserPermissionInGroup = _accessMenuService.SelectUserPermissionInGroup(menuID, CurrentUser.UserID);
            //var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(menuID, CurrentUser.UserID);

            //if (listUserPermissionInGroup.Count > 0) // check user Permission in Group
            //{
            //    if (listUserPermissionInGroup[0].SAVE_YN_SUM > 0)
            //    {
            //        model.Btn_Save = true;
            //    }
            //}
            //else if (listUserPermissionAccessMenu.Count > 0) //check user Permission
            //{
            //    if (listUserPermissionAccessMenu[0].SAVE_YN == true)
            //    {
            //        model.Btn_Save = true;
            //    }
            //}
            //ViewBag.Thread = Guid.NewGuid().ToString("N");
            //ViewBag.ProjectName = model.ProjectName;
            //ViewBag.ProjectCode = model.ProjectCode;
            //ViewBag.UserProjectCode = model.UserProjectCode;
            //ViewBag.requestCode = model.RequestCode;
            //ViewBag.PartnerName = model.PartnerName;
            //ViewBag.ProjectStatus = model.ProjectStatus;
            //ViewBag.ProjectStatusName = model.ProjectStatusName;
            //ViewBag.RequestDate = model.RequestDate;
            //ViewBag.RequestDate = model.RequestDate;
            //ViewBag.OrderQuantity = model.OrderQuantity;
            //ViewBag.RequestMessage = model.RequestMessage;
            //ViewBag.ItemName = model.ItemName;
            //ViewBag.ItemCode = model.ItemCode;
            //ViewBag.RequestType = model.RequestType;
            //ViewBag.UserRequest = CurrentUser.UserName;
            //ViewBag.MenuID = menuID;
            //ViewBag.Parent = vbParent;
            return Json(model.ProjectStatus);
        }
        public IActionResult ReloadRequestMst(string projectCode, int menuID)
        {
            var model = _mesProductionRequestService.GetDataDetail(projectCode);
            return Content(JsonConvert.SerializeObject(model), "application/json");
        }
        // Quan add 2020/09/11
        // Get list Item Request in grid
        [HttpGet]
        // Get list Material
        [HttpGet]
        public IActionResult GetListItemMaterialNotexist(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var data = _mesItemService.GetListItemMaterialNotexist(projectCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Quan add 2020/09/11
        // Get list Item Request
        [HttpGet]
        public IActionResult GetlistItemRequest(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var listItemRequest = _mesProductionRequestService.GetListItemRequest(projectCode);
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
           
            var listItemRequest = _mesProductionRequestService.GetListItemRequest(projectCode);

            if( listItemRequest.Count == 0)
            {
                return Json("1");
            }
            else
            {
                // Quan change logic 2021-01-25
                //var ResultCheck = listItemRequest.Where(x => x.ReqQty * orderQuantity > x.RealQty).FirstOrDefault();
                //var ResultCheckRealQty = listItemRequest.Where(x => x.ReqQty == 0 || x.RealQty == null).FirstOrDefault();
                //if (ResultCheck != null || ResultCheckRealQty != null)
                //{

                //    return Json("1");
                //}
                //else
                //{
                //    return Json(null);
                //}
                return Json(null);
            }
        }
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list Common Code by groupCode
        [HttpGet]
        public IActionResult GetListCommonCode(string groupCode)
        {
            var data = _mesComCodeService.GetListComCodeDTL(groupCode);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // Quan add
        // Get list Common Code Category IMTP01, IMTP02
        [HttpGet]
        public IActionResult GetListComCodeCategoryMaterial(string groupCD,string baseCD1, string baseCD2, string baseCD3, string baseCD4)
        {
            var data = _mesComCodeService.GetListComCodeCategoryMaterial(groupCD, baseCD1, baseCD2, baseCD3, baseCD4);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // Get list Material
        [HttpGet]
        public IActionResult GetListMaterial(DataSourceLoadOptions loadOptions)
        {
            //int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
            //int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

            //var data = mESItemService.GetListData(pageSize, itemSkip);
            var data = _mesItemService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list Material by ItemClassCode
        [HttpGet]
        public IActionResult GetListMaterialByItemClassCode(DataSourceLoadOptions loadOptions, string ItemClassCode)
        {
            var data = _mesItemService.GetListMaterialByItemClassCode(ItemClassCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get Material detail
        [HttpGet]
        public IActionResult GetDetailMaterial(string itemCode)
        {
            var data = _mesItemService.GetDetail(itemCode);
            return Json(data);
        }

        // Get list Partner by itemCode
        [HttpGet]
        public IActionResult GetListPartnerByItemCode(string itemCode)
        {
            var data = _mesItemPartnerService.GetListPartnerByItem(itemCode);
            return Json(data);
        }

        // Get item detail of Partner
        [HttpGet]
        public IActionResult GetDetailItemOfPartner(string itemCode, string partnerCode)
        {
            var data = _mesItemPartnerService.GetItemDetail(itemCode, partnerCode);
            return Json(data);
        }

        // Get link file sale project
        [HttpGet]
        public IActionResult GetListFileUrlSaleProject(string projectCode)
        {
            var result = _mesSaleProjectService.GetListFileUrlSaleProject(projectCode);
            return Json(result);
        }

        // Get link file sale project
        [HttpPost]
        public IActionResult SaveUrlFile(int Id, string saleProjectID, string fileUrl, int flag)
        {
            var result = _mesSaleProjectService.SaveUrlFile(Id, saleProjectID, fileUrl, flag);
            return Json(result);
        }


        [HttpGet]
        public IActionResult GetListFile(string fileID)
        {
            var result = _mesSaleProjectService.GetSYFileUploadByID(fileID);
            return Json(result);
        }

        #endregion

        #region "Insert - Update - Delete"

        // Save link file 
        // Thien add 2022-01-19
        [HttpPost]
        public  IActionResult SaveListFile(string updateData, string projectCode)
        {
            List<MES_UrlByUser> updateDataList = JsonConvert.DeserializeObject<List<MES_UrlByUser>>(updateData);
            var result = _mesSaleProjectService.SaveListFile(updateDataList, projectCode);
            return Json(result);
        }



        // Save data Production Request
        // Save data list Itemquest
        [HttpPost]
        public IActionResult SaveDataProductionRequestChange(string projectCode, string requestCode, string requestType, string userIDRequest, string requestDate, string requestMessage, List<ItemRequest> listItemRequest)
        {
            //listItemRequest.RemoveAt(0);          
            string RequestCode = string.IsNullOrEmpty(requestCode) ? "PQ" + projectCode.Substring(2, projectCode.Length - 2) : requestCode;

            var result = _mesProductionRequestService.SaveDataProductionRequestChange(projectCode, RequestCode, requestType, CurrentUser.UserID, requestDate, requestMessage, listItemRequest);
            return Json(result);
        }
        public IActionResult UpdateDataProductionRequestChange(string projectCode, string requestCode, string requestType, string userIDRequest, string requestDate, string requestMessage, string ProductType, string ItemCode, bool InitialCode)
        {

            var model = new MES_SaleProject();
            string RequestCode = string.IsNullOrEmpty(requestCode) ? "PQ" + projectCode.Substring(2, projectCode.Length - 2) : requestCode;

            var result = _mesProductionRequestService.UpdateDataProductionRequestChange(projectCode, RequestCode, requestType, CurrentUser.UserID, requestDate, requestMessage, ProductType, ItemCode, InitialCode);
            return Json(result);
        }
        // Quan add 2020/09/14
        // Send Production Request
        [HttpPost]
        public IActionResult RequestProduction(string projectCode,string requestDate,string RequestType)
        {
            var result = _mesProductionRequestService.RequestProduction(projectCode, CurrentUser.UserID, requestDate, RequestType);
            return Json(result);
        }
        [HttpPost]
        public IActionResult DeleteGridItemPartList(string ItemCode,string RequestCode)
        {
            var result = _mesProductionRequestService.DeleteGridItemPartList(ItemCode, RequestCode);
            return Json(result);
        }
        // Quan add 2021-04-14
        // Delete DeleteBatchRowItem
        [HttpPost]
        public IActionResult DeleteBatchRowItem(string ListItem, string RequestCode)
        {
            var result = _mesProductionRequestService.DeleteBatchRowItem(ListItem, RequestCode);
            return Json(result);
        }
        // Quan add 2020/09/16
        // GetI tem ByProject Code In Status ('PJST03','PJST04','PJST05','PJST06')
        [HttpGet]
        public IActionResult GetItemByProjectCodeInStatus(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetItemByProjectCodeInStatus();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Quan add 2021/09/16
        // GetI tem ByProject Code In Status ('PJST03','PJST04','PJST05','PJST06')
        [HttpGet]
        public IActionResult GetProjectGoodsDelivery(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetProjectGoodsDelivery();
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
            var result = _mesProductionRequestService.RecallProductionRequest(projectCode, CurrentUser.UserID);
            return Json(result);
        }
        #endregion

        #region Show Popup Item Related To Project

        [HttpGet]
        public IActionResult ProductionRequestPopup(string projectCode, string vbThread)
        //public IActionResult ProductionRequestPopup()
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();

            //if (projectCode != null)
            //{
            //    model = mESProductionRequestService.GetDataDetail(projectCode);
            //}
            return PartialView("ProductionRequestPopup", model);
        }

        [HttpGet]
        public IActionResult GetBOMData(string projectCode)
        {
            var result = _mesBOMMgtService.GetBOMItemBySalePJCode(projectCode);
            return Json(result);
        }
        public IActionResult ShowProjectNamePopup(string idParent)
        {
            var model = new MES_SalesOrderProjectNew();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("ProjectNamePoupup", model);
        }
        [HttpGet]
        public IActionResult ProductionRequestAddPartListPopup(string projectCode, string vbThread)
        //public IActionResult ProductionRequestPopup()
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();

            //if (projectCode != null)
            //{
            //    model = mESProductionRequestService.GetDataDetail(projectCode);
            //}
            return PartialView("ProductionRequestAddPartListPopup", model);
        }

        [HttpGet]
        public IActionResult GetCategory(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetListCategories();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemClass(DataSourceLoadOptions loadOptions, string categoryCode)
        {
            var data = _mesProductionRequestService.GetListItemClass(categoryCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfProject(string ProjectCode, string Category, string ItemClassCode, string ItemCode)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = _mesProductionRequestService.GetListItemOfProject(ProjectCode, ItemCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfItemClasscode(string Category, string ItemClassCode, string ItemCode, string ItemName)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = _mesProductionRequestService.GetListItemOfItemClassCode(Category, ItemClassCode, ItemCode, ItemName);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        [HttpGet]
        public IActionResult GetListItemFromQrScanning(string ItemClassCode, string ItemCode)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = _mesProductionRequestService.GetListItemFromQrScanning(ItemClassCode, ItemCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        #endregion


        [HttpGet]
        public IActionResult GetListProductionLine(string projectCode)
        {
            var result = _mesProductionRequestService.GetListProductionLine(projectCode);
            return Json(result);
        }

        [HttpGet]
        public IActionResult showPopupGetItem(string idParent)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("PopupGetItem", model);
        }

        public IActionResult CheckPlanRequestQty(string projectCode)
        {
            var check = _mesProductionRequestService.CheckPlanRequestQty(projectCode);
            
            return Json(check);
        }
    }

}
