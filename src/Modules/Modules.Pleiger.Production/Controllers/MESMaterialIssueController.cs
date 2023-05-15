using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Extensions;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.FileUpload.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.Production.Model;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Modules.Pleiger.Production.Controllers
{
    public class MESMaterialIssueController : BaseController
    {
        private readonly IMESMaterialIssueService _MESMaterialIssueService;
        private readonly IMESBOMMgtService _mesBOMMgtService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESProductionService productionService;
        private readonly IMESProductionRequestService _mesProductionRequestService;
        private readonly IUserService _userService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUploadFileWithTemplateService _uploadFileWithTemplateService;

        public MESMaterialIssueController(IMESMaterialIssueService MESMaterialIssueService,
                                          IMESBOMMgtService mesBOMMgtService, IHttpContextAccessor contextAccessor,
                                          IMESProductionRequestService mesProductionRequestService,
                                          IUserService userService, IAccessMenuService accessMenuService,
                                          IUploadFileWithTemplateService uploadFileWithTemplateService, 
                                          IMESProductionService productionService) : base(contextAccessor)
        {
            this._MESMaterialIssueService = MESMaterialIssueService;
            this._mesBOMMgtService = mesBOMMgtService;
            this._contextAccessor = contextAccessor;
            this._mesProductionRequestService = mesProductionRequestService;
            this._userService = userService;
            this._accessMenuService = accessMenuService;
            this._uploadFileWithTemplateService = uploadFileWithTemplateService;
            this.productionService = productionService;
        }

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.DetailThread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;

            var listUserPermissionInGroup = _accessMenuService.SelectUserPermissionInGroup(menuID, CurrentUser.UserID);
            var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(menuID, CurrentUser.UserID);
            SYMenuAccess pageSetting = new SYMenuAccess();
            if (listUserPermissionInGroup.Count > 0) // check user Permission in Group
            {
                pageSetting.IMPORT_EXCEL_YN = listUserPermissionInGroup[0].EXCEL_YN;
                pageSetting.PRINT_YN = listUserPermissionInGroup[0].PRINT_YN;
                pageSetting.CREATE_YN = listUserPermissionInGroup[0].CREATE_YN;
                pageSetting.EXCEL_YN = listUserPermissionInGroup[0].EXCEL_YN;
                pageSetting.SEARCH_YN = listUserPermissionInGroup[0].SEARCH_YN;
            }
            if (listUserPermissionAccessMenu.Count > 0) //check user Permission
            {
                pageSetting.IMPORT_EXCEL_YN = listUserPermissionAccessMenu[0].EXCEL_YN;
                pageSetting.PRINT_YN = listUserPermissionAccessMenu[0].PRINT_YN;
                pageSetting.CREATE_YN = listUserPermissionAccessMenu[0].CREATE_YN;
                pageSetting.EXCEL_YN = listUserPermissionAccessMenu[0].EXCEL_YN;
                pageSetting.SEARCH_YN = listUserPermissionAccessMenu[0].SEARCH_YN;
            }
            ViewBag.PageSetting = pageSetting;

            return View();
        }

        [HttpGet]
        public IActionResult SearchMaterialIssue(string MaterialIssueNo, string StartDate, string EndDate, string UseTeam, string ProductionProjectCode, string MaterialIssueStatus)
        {
            var result = _MESMaterialIssueService.searchMaterialIssue(CurrentLanguages.Substring(1), MaterialIssueNo, StartDate, EndDate, UseTeam, ProductionProjectCode, MaterialIssueStatus);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public IActionResult MaterialIssueDetail(string MaterialIssueNo, string SaveAction, int menuID, string ParentThread, string DetailThread)
        {
            var listUserPermissionInGroup = _accessMenuService.SelectUserPermissionInGroup(menuID, CurrentUser.UserID);
            var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(menuID, CurrentUser.UserID);

            SYMenuAccess pageSetting = new SYMenuAccess();
            if (listUserPermissionInGroup.Count > 0) // check user Permission in Group
            {
                pageSetting.SAVE_YN = listUserPermissionInGroup[0].SAVE_YN;
                pageSetting.IMPORT_EXCEL_YN = listUserPermissionInGroup[0].EXCEL_YN;
                pageSetting.PRINT_YN = listUserPermissionInGroup[0].PRINT_YN;
            }
            if (listUserPermissionAccessMenu.Count > 0) //check user Permission
            {
                pageSetting.SAVE_YN = listUserPermissionAccessMenu[0].SAVE_YN;
                pageSetting.IMPORT_EXCEL_YN = listUserPermissionAccessMenu[0].EXCEL_YN;
                pageSetting.PRINT_YN = listUserPermissionAccessMenu[0].PRINT_YN;
            }
            ViewBag.PageSetting = pageSetting;
            ViewBag.Thread = DetailThread;
            ViewBag.MenuId = menuID;
            ViewBag.ParentThread = ParentThread;

            ViewBag.SaveAction = SaveAction;
            ViewBag.currentUserId = CurrentUser.UserID;
            ViewBag.currentUsername = CurrentUser.UserName;
            ViewBag.qrCodeString = "";
            ViewBag.qrCountSemicolum = 0;


            if (SaveAction == "Insert")
            {
                return View(new MESMaterialIssue());
            }
            var mESMaterialIssue = _MESMaterialIssueService.getMaterialIssueByNo(MaterialIssueNo, CurrentLanguages.Substring(1));
            ViewBag.SlipNumber = mESMaterialIssue.SlipNumber;
            ViewBag.StatusCode = mESMaterialIssue.StatusCode;
            return View(mESMaterialIssue);
        }

        [HttpPost]
        public IActionResult MaterialIssuePopupImportExcel(MESMaterialIssue MESMaterialIssue, string pageParentThread)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.ParentThread = pageParentThread;
            ViewBag.mESMaterialIssue = MESMaterialIssue;
            return PartialView("MESMaterialIssueImportExcel");
        }

        [HttpPost]
        public IActionResult showPopupGetItem(string pageParentThread)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.pageThread = pageParentThread;
            return PartialView("PopupGetItem");
        }

        [HttpPost]
        public IActionResult insertIssuesData(string UseTeam, string ItemCode, string MaterialIssueComment, string issueDate, string ProductionProjectCode,string CreateDate)
        {
            var currentUserId = CurrentUser.UserID;
            string status = "MIS001";
            var result = _MESMaterialIssueService.saveMaterialIssue("", issueDate, UseTeam, status, "", ItemCode, ProductionProjectCode, currentUserId, MaterialIssueComment, CreateDate);
            string[] data = result.ToString().Split("-");
            //var data = JsonConvert.SerializeObject(txt);
            MES_SlipAndIssueNumber slip = new MES_SlipAndIssueNumber
            {
                SlipNumber = data[1],
                MaterialIssue = data[0],
            };
            return Json(slip);
        }

        [HttpPost]
        public IActionResult updateIssuesData(string MaterialIssueNo, string MaterialIssueDate, string UseTeam, string ItemCode, string MaterialIssueComment, string Status, string ProductionProjectCode,string CreateDate)
        {
            var result = _MESMaterialIssueService.saveMaterialIssue(MaterialIssueNo, MaterialIssueDate, UseTeam, Status, "", ItemCode, ProductionProjectCode, "", MaterialIssueComment, CreateDate);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpPost]
        public IActionResult updateIssuesStatus(string MaterialIssueNo, string Status)
        {
            var result = _MESMaterialIssueService.updateStatus(MaterialIssueNo, Status);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public object LoadProjectCodeCombobox(DataSourceLoadOptions loadOptions)
        {
            var result = _MESMaterialIssueService.getProjectCodeCombobox();
            return DataSourceLoader.Load(result, loadOptions);
        }

        [HttpGet]
        public IActionResult GetBOMData(string ItemCode)
        {
            var result = _MESMaterialIssueService.GetBOMItem(ItemCode);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public IActionResult GetItemFromsProjectPopup(string projectCode, string vbThread)
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();
            return PartialView("GetItemFromsProjectPopup", model);
        }

        [HttpGet]
        public IActionResult MaterialIssueAddPartListPopup(string projectCode, string vbThread)
        {
            ViewBag.OldThread = vbThread;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_SaleProject();
            return PartialView("MaterialIssueAddPartListPopup", model);
        }

        [HttpGet]
        public IActionResult getPartList(string MaterialIssueNo)
        {
            var result = _MESMaterialIssueService.getMaterialIssuePartList(MaterialIssueNo);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpPost]
        public ActionResult UploadFilePopup(IFormFile myFile, string chunkMetadata)
        {
            try
            {
                Type myType = Type.GetType("Modules.Pleiger.Production.Model.MESIssueItemPart");

                var result = _uploadFileWithTemplateService.UploadFileDynamicForImportFromPopup(myFile, chunkMetadata, myType);

                return Json(new { result = true, data = result });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("DateTime", StringComparison.Ordinal))
                {
                    Result result = new Result();
                    result.Success = false;
                    if (CurrentLanguages == "en")
                    {
                        result.Message = MessageCode.MD00014_EN;
                    }
                    else
                    {
                        result.Message = MessageCode.MD00014_KR;
                    }
                    return Json(result);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost]
        public IActionResult savePartListData(string MaterialIssueNo, string Data, string ItemToDelete, string SlipNumber)
        {
            var partList = _MESMaterialIssueService.getMaterialIssuePartList(MaterialIssueNo);
            List<MESIssueItemPart> partChanged = new List<MESIssueItemPart>();
            List<MESIssueItemPart> newPartList = JsonConvert.DeserializeObject<List<MESIssueItemPart>>(Data);
            newPartList.ForEach(newItem =>
            {
                partList.ForEach(item =>
                {
                    if(item.ItemCode == newItem.ItemCode)
                    {
                        newItem.ReqQty -= item.ReqQty;
                        partChanged.Add(newItem);
                    }
                });
            });
            var stringNewPartList = partList.Count == 0 ? Data : JsonConvert.SerializeObject(partChanged);
            var currentUserId = CurrentUser.UserID;
            var result = _MESMaterialIssueService.savePartList(MaterialIssueNo, stringNewPartList, ItemToDelete, SlipNumber, currentUserId);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        //[HttpPost]
        //public IActionResult importPartListExcelFile(string filePath, string materWHCode)
        //{
        //    string result = _MESMaterialIssueService.partListExcelFileToJson(filePath, CurrentLanguages);
        //    var dataConvert = JsonConvert.SerializeObject(result);
        //    var data = _MESMaterialIssueService.UpdateItemCodeInWH(dataConvert, materWHCode);
        //    return Content(data, "application/json");
        //}

        [HttpGet]
        public ActionResult UpdateDataFromExcelSaleProject(string fileLoc, string materWHCode, string materialIssueNo)
        {
            var result = _MESMaterialIssueService.UpdateItemToWH(fileLoc, materWHCode, CurrentLanguages, materialIssueNo);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetItemByProjectCodeInStatus(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetItemByProjectCodeInStatus();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetCategory(DataSourceLoadOptions loadOptions)
        {
            var data = _mesProductionRequestService.GetListCategories();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfProject(string ProjectCode, string Category, string ItemClassCode, string ItemCode)
        {
            var result = _mesProductionRequestService.GetListItemOfProject(ProjectCode, ItemCode);
            List<MESIssueItemPart> data = new List<MESIssueItemPart>();
            foreach (var item in result)
            {
                var part = new MESIssueItemPart()
                {
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ReqQty = item.RealQty,
                    StkQty = item.StkQty,
                    RealQty = item.RealQty,
                    CategoryName = item.CategoryName,
                    ItemClassCode = item.ItemClassCode,
                    Note = ""
                };
                data.Add(part);
            }
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemClass(DataSourceLoadOptions loadOptions, string categoryCode)
        {
            var data = _mesProductionRequestService.GetListItemClass(categoryCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListItemOfItemClasscode(string Category, string ItemClassCode, string ItemCode, string ItemName)
        {
            //var data = mESProductionRequestService.GetListItemOfProject(ProjectCode, Category, ItemClassCode, ItemCode);
            var data = _mesProductionRequestService.GetListItemOfItemClassCode(Category, ItemClassCode, ItemCode, ItemName);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        [HttpGet]
        public IActionResult GetItemFromProductionCode(string productionCode)
        {
            var model = productionService.GetDetailData(productionCode, "PJST03");

            return Content(JsonConvert.SerializeObject(model), "application/json");
        }
    }
}
