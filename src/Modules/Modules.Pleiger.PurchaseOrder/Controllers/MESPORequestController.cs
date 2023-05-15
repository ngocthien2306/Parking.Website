using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using InfrastructureCore.Utils;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.SalesProject.Services.IService;
using Modules.Admin.Services.IService;
using Modules.Pleiger.CommonModels;
using System.IO;
using Modules.Common.Models;
using InfrastructureCore;
using InfrastructureCore.RapidReport;
using System.Data;
using Modules.Admin.Models;
using InfrastructureCore.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Globalization;
using Modules.Pleiger.FileUpload.Services.IService;

namespace Modules.Pleiger.PurchaseOrder.Controllers
{
    public class MESPORequestController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IPORequestService pORequestService;
        private readonly IMESComCodeService mESComCodeService;
        private readonly IMESItemPartnerService mESItemPartnerService;
        private readonly IPurchaseService _purchaseService;
        private readonly IMESPartnerService _partnerService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUserService _userService;
        private readonly ISendMailServices _sendMailServices;
        private readonly IRazorViewRenderer _razorViewRenderer;
        private readonly IConfiguration _config;
        private readonly IUploadFileWithTemplateService _uploadFileWithTemplateService;

        public MESPORequestController(
            IHttpContextAccessor contextAccessor,
            IAccessMenuService accessMenuService,
            IPORequestService pORequestService,
            IMESComCodeService mESComCodeService,
            IMESItemPartnerService mESItemPartnerService,
            IPurchaseService purchaseService,
            IMESPartnerService partnerService,
            ISendMailServices sendMailServices,
            IRazorViewRenderer razorViewRenderer,
            IConfiguration config,
            IMESSaleProjectService mesSaleProjectService,
            IUploadFileWithTemplateService uploadFileWithTemplateService,
            IUserService userService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.pORequestService = pORequestService;
            this.mESComCodeService = mESComCodeService;
            this.mESItemPartnerService = mESItemPartnerService;
            this._purchaseService = purchaseService;
            this._partnerService = partnerService;
            this._mesSaleProjectService = mesSaleProjectService;
            this._accessMenuService = accessMenuService;
            this._userService = userService;
            this._sendMailServices = sendMailServices;
            this._razorViewRenderer = razorViewRenderer;
            this._config = config;
            this._uploadFileWithTemplateService = uploadFileWithTemplateService;

        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.MenuIDParent = CurrentMenu.MenuID;
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            if (CurrentUser == null)
            {
                return NotFound();
            }
            var SelectUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(CurrentMenu.MenuID, CurrentUser.UserID);
            if (SelectUserPermissionAccessMenu.Count > 0)
            {
                ViewBag.PurchaseYN = SelectUserPermissionAccessMenu[0].PURCHASE_ORDER_YN;
            }
            else
            {
                ViewBag.PurchaseYN = false;
            }
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;

            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);

            var partnerCode = "";
            ViewBag.CheckUserType = "";
            // Partner in groupcode SY Comcode => to disable create button
            if (checkUserLogin.SystemUserType.Equals("G000C005") || checkUserLogin.SystemUserType.Equals("G000C007") || checkUserLogin.SystemUserType.Equals("G000C006"))
            {
                var checkUserEmployee = new CHECKRESULT();
                checkUserEmployee = _userService.CheckUserEmployee(checkUserLogin.UserCode);
                if (checkUserEmployee != null) /// có trong table employ
                {
                    partnerCode = checkUserEmployee.PartnerCode;
                }
                else
                {
                    partnerCode = checkUserLogin.UserCode;
                }
                ViewBag.CheckUserType = checkUserLogin.SystemUserType;
            }
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;


            // case check must be partner code and country to show grid
            // MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(checkUserLogin.UserCode);
            MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(partnerCode);
            if (checkPartner != null)
            {
                if (checkPartner.CountryType.Equals("CTTP01"))
                {
                    ViewBag.partnerCountry = "CTTP01";
                }
                else if (checkPartner.CountryType.Equals("CTTP02"))
                {
                    //oversea partner
                    ViewBag.partnerCountry = "CTTP02";
                }
            }
            else
            {
                ViewBag.partnerCountry = "";
            }

            return View();
        }

        // Get list PO Request
        [HttpGet]
        public IActionResult GetListData()
        {
            var data = pORequestService.GetListData();
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Search list PO Request
        [HttpGet]
        public IActionResult SearchListPORequest(string projectCode, string poNumber, string userPONumber, string projectName,
            string requestDateFrom, string requestDateTo, string poStatus, string SalesClassification, string PartnerCode)
        {
            string partnerCode = "";
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            //If User login = Partner and customer
            var checkUserEmployee = new CHECKRESULT();

            if (checkUserLogin.SystemUserType.Equals("G000C005") || checkUserLogin.SystemUserType.Equals("G000C007") || checkUserLogin.SystemUserType.Equals("G000C006")) //them 007 
            {
                if (checkUserLogin != null)
                {
                    checkUserEmployee = _userService.CheckUserEmployee(checkUserLogin.UserCode);
                    if (checkUserEmployee != null) /// có trong table employ
                    {
                        partnerCode = checkUserEmployee.PartnerCode;
                    }
                    else
                    {
                        partnerCode = checkUserLogin.UserCode;
                    }
                }
                var data = pORequestService.SearchListPORequest(projectCode, poNumber, userPONumber, projectName, requestDateFrom, requestDateTo, partnerCode, poStatus, SalesClassification, true);
                return Content(JsonConvert.SerializeObject(data), "application/json");
            }
            else //If pleiger user and Admin 
            {
                var data = pORequestService.SearchListPORequest(projectCode, poNumber, userPONumber, projectName, requestDateFrom, requestDateTo, PartnerCode, poStatus, SalesClassification, false);
                return Content(JsonConvert.SerializeObject(data), "application/json");
            }
        }

        //Thien add 2022-01-28 - show history delivery item
        [HttpGet]
        public IActionResult ShowHistoryDeliveryDateItem()
        {
            return Json("");
        }
        // Show detail PO Request
        [HttpGet]
        public IActionResult ShowDetailPORequest(string projectCode, string poNumber, string partnerCode, int menuIDParent, string viewbagParent)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.menuIDParent = menuIDParent;
            ViewBag.Parent = viewbagParent;
            var SelectUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(menuIDParent, CurrentUser.UserID);
            if (SelectUserPermissionAccessMenu.Count > 0)
            {
                ViewBag.PurchaseYN = SelectUserPermissionAccessMenu[0].PURCHASE_ORDER_YN;
            }
            else
            {
                ViewBag.PurchaseYN = false;
            }
            var model = new MES_PORequest();
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            if (projectCode != null || poNumber != null)
            {
                model = pORequestService.GetPODetail(poNumber); //lay thang cha
                var listItemPORequest = pORequestService.GetListItemPORequest(poNumber);
                foreach (var item in listItemPORequest)
                {
                    ViewBag.MonetaryDisplayInGrid = item.MonetaryUnit;
                    ViewBag.PrintingStatus = item.PrintingStatus;
                    break;
                }
                try
                {
                    model.ListItemPORequest = listItemPORequest;
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }

                model.PartnerCode = partnerCode;
                //check partner code => over sea or normal => then set flag to render POoversea html
                MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(partnerCode);
                //them case null
                if (checkPartner != null)
                {
                    if (checkPartner.CountryType.Equals("CTTP01"))
                    {
                        model.partnerCountry = "CTTP01";
                    }
                    else if (checkPartner.CountryType.Equals("CTTP02"))
                    {
                        //oversea partner
                        model.partnerCountry = "CTTP02";
                    }
                }
                else
                {
                    model.partnerCountry = "";
                }

                model.State = "Detail";
                object listPartnerDTL = GetListPartner(projectCode); //when get detail of PO => send a list partner to detail screen
                ViewBag.listPartnerDTL = listPartnerDTL != null ? listPartnerDTL : "";
                ViewBag.SystemUserType = checkUserLogin.SystemUserType;
            }
            else
            {
                model.State = "Create";
                model.IsNew = true;
                model.UserRequest = CurrentUser.UserName;
                model.RequestDate = DateTime.Now.ToString("yyyy-MM-dd");
                model.ListItemPORequest = new List<MES_ItemPO>();
                model.partnerCountry = ""; //rong la ko hien over sea
                ViewBag.SystemUserType = checkUserLogin.SystemUserType;
                model.StatusCode = null;

            }
            ViewBag.Remark = model.RemarkYN;

            return PartialView("DetailNewPORequest", model);
        }
        // Get list Lead Time Type

        [HttpGet]
        public IActionResult GetListLeadTimeType()
        {
            var data = mESComCodeService.GetListComCodeDTL("LDTM00");
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        public List<MES_SaleProject> SearchProjectCode(string itemCode, string itemName,string productionProjectCode ,string productionProjectName, string projectType)
        {
            var data = pORequestService.SearchProjectCodeByParams(itemCode, itemName, productionProjectCode, productionProjectName, projectType);
            return data;
        }

        [HttpGet]
        public List<MES_SaleProject> GetProjectCodeByStatus()
        {
            var data = pORequestService.GetProjectCodeByStatus();
            return data;
        }

        // Show history update
        [HttpGet]
        public IActionResult ShowPopupHistoryEditDelivery(string PoNumber, string ItemCode, string UserPONumber)
        {

            ViewBag.PONumber = PoNumber;
            ViewBag.ItemCode = ItemCode;
            ViewBag.UserPONumber = UserPONumber;
            return PartialView("ShowHistoryUpdateItem");
        }
        [HttpGet]
        public IActionResult GetListHistoryDeliveryDateItem(string PoNumber, string ItemCode, string UserPONumber)
        {
            var result = pORequestService.GetListHistoryUpdateDeliveryItem(PoNumber, ItemCode, CurrentUser.UserID);
            return Json(result);
        }
        #endregion

        #region "Insert - Update - Delete"

        [HttpPost]

        public IActionResult SaveDataPORequest(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.SavePO(flag, PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }
        [HttpPost]

        public IActionResult SaveDataPOMstRequest(string flag, MES_PORequest PORequest)
        {
            //find POMst 
            MES_PORequest POMst = pORequestService.GetPODetail(PORequest.PONumber);
            List<MES_ItemPO> listDetails = pORequestService.GetListItemPORequest(POMst.PONumber);
            var result = pORequestService.SavePO(flag, POMst, listDetails, CurrentUser.UserID);
            return Json(result);
        }
        [HttpGet]

        public IActionResult CheckPartnerOverSea(string partnerCode, string threadId)
        {
            MES_Partner model = pORequestService.getPartnerByPartnerCode(partnerCode);
            ViewBag.Thread = threadId;
            return Json(model);
        }

        public IActionResult CheckOverSeaByPartnerCode(string partnerCode)
        {
            MES_Partner model = pORequestService.getPartnerByPartnerCode(partnerCode);
            return Json(model);
        }
        [HttpGet]

        public IActionResult GetOverSeaDetail(string projectCode, string poNumber, string partnerCode, string threadId)
        {
            var model = new MES_PORequest();
            MES_Partner partner = pORequestService.getPartnerByPartnerCode(partnerCode);
            if (projectCode != null || poNumber != null)
            {
                model = pORequestService.GetPODetail(poNumber);
                var listItemPORequest = pORequestService.GetListItemPORequest(poNumber);
                model.ListItemPORequest = listItemPORequest;
                model.IsNew = false;
                model.PartnerCode = partnerCode;
                if (partner.CountryType.Equals("CTTP01"))
                {
                    partner.CountryType = "CTTP01";
                }
                else if (partner.CountryType.Equals("CTTP02"))
                {
                    partner.CountryType = "CTTP02";
                }
            }
            ViewBag.Thread = threadId;
            ViewBag.PORequest = model;
            return PartialView("POOverSea", partner);
        }
        //update PO detail partner
        [HttpPost]

        public IActionResult UpdatePODetailpartner(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.UpdatePODetailPartner(flag, PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }
        public IActionResult UpdatePODetailpartnerETA(MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.UpdatePODetailPartnerETA(PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }
        
        [HttpPost]
        public IActionResult UpdateStatusPartner(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.UpdateStatusPoRemarkItem(flag, PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }

        [HttpPost]
        public IActionResult UpdateStatusChangedDayPartner(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.UpdateStatusPoChangedDayItem(flag, PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }


        [HttpPost]
        public IActionResult UpdateHistoryDeliveryDate(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.UpdateHistoryDeliveryDateItem(flag, PORequest, listDetails, CurrentUser.UserID);
            return Json(result);
        }
        [HttpPost]
        public IActionResult UpdateStatusPartnerToClosePopup(string PoNumber, string flag)
        {
            var result = new Result();
            List<MES_PORequest> listDetails = JsonConvert.DeserializeObject<List<MES_PORequest>>(PoNumber);
            foreach (var item in listDetails)
            {
                result = pORequestService.UpdateStatusPartnerToClosePopup(item.PONumber, flag);
            }

            return Json(result);
        }
        // Quan add 2020/09/28
        // Delete POMst
        [HttpPost]
        public IActionResult DeletePOMst(string PONumber)
        {
            var result = pORequestService.DeletePOMst(PONumber);
            return Json(result);
        }

        #endregion
        #region Print PO TO PDF FILE

        [HttpGet]
        public async Task<IActionResult> ExportExcelPO(string PONumber)
        {
            //var data = await pORequestService.CreatePOPrint(PONumber);
            var data = await pORequestService.ExportExcelPO(PONumber);

            var memory = new MemoryStream();
            using (var stream = new FileStream(data, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(true);
            }
            memory.Position = 0;
            var file = await Task.Run(() => File(memory, ExcelExport.GetContentType(data), data.Remove(0, data.LastIndexOf("\\") + 1))).ConfigureAwait(true);
            return Json(new { downloadExcelPath = data, fileName = file.FileDownloadName });
        }

        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";
        private const string EXCEL_FILE_NAME = "PORequest";
        public ActionResult GetDetailPOReq(string poNumber)
        {
            var PDFDetail = pORequestService.GetPODetailDataPrint(poNumber);
            string partnerCountry = "CTTP01";

            if (PDFDetail.Count > 0)
            {
                partnerCountry = PDFDetail.FirstOrDefault().partnerCountry;

                string PDFFileName = $"{EXCEL_FILE_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.rrpt";
                Result result = new Result();
                RapidReportModule rapid = new RapidReportModule();
                string ouputfilePath = "";
                DataTable dataTable = ExcelExport.ConvertToDataTable(PDFDetail);
                try
                {
                    if (partnerCountry == "CTTP01")
                    {
                        ouputfilePath = rapid.Run(dataTable, "PurchaseOrder_kr", "PORequest", "En");
                    }
                    else
                    {
                        ouputfilePath = rapid.Run(dataTable, "PurchaseOrder_en", "PORequest", "En");
                    }
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message.ToString();
                    result.Success = false;
                    return Json(new { Result = result });
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(ouputfilePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                var file = File(memory, ExcelExport.GetContentType(ouputfilePath), ouputfilePath.Remove(0, ouputfilePath.LastIndexOf("\\") + 1));
                result.Success = true;
                return Json(new { Result = result, downloadExcelPath = ouputfilePath, fileName = file.FileDownloadName });

            }
            return Json(new { Result = false, downloadExcelPath = "", fileName = "" });

        }
        [HttpGet]
        public async Task<ActionResult> Download(string fileLink, string fileName)
        {
            Debug.WriteLine("PVN Test 6 ");
            Debug.WriteLine("Download Run At " + DateTime.Now);
            if (fileName != null)
            {

                string Files = fileLink;

                var a = System.IO.File.ReadAllBytes(Files);
                byte[] fileBytes = a;
                await Task.Run(() => System.IO.File.WriteAllBytes(Files, fileBytes)).ConfigureAwait(true);
                MemoryStream ms = new MemoryStream(fileBytes);
                return await Task.Run(() => File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName)).ConfigureAwait(true);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }

        #endregion

        #region bao add
        [HttpGet]
        public object GetListPartner(string projectcode)
        {
            var listfullpartner = _partnerService.getListPartner_ByProjectCode(projectcode);
            //var distinctset = new list<mes_itempartner>();
            //foreach (var item in listfullpartner)
            //{
            //    if (!distinctset.any(x => x.partnercode == item.partnercode))
            //    {
            //        distinctset.add(item);
            //    }
            //}
            //var listpartner = new mes_itempartnerresult()
            //{
            //    listdistinctpartner = distinctset,
            //    listfullpartner = listfullpartner
            //};
            return listfullpartner;
        }
        [HttpGet]
        public object GetListPartnerNoParams()
        {
            var listFullPartner = _partnerService.GetAllPartner();
            return listFullPartner;
        }
        [HttpGet]
        public object ShowPopupItemsPartner(string partnerCode, string threadId)
        {
            ViewBag.Thread = threadId;
            ViewBag.PartnerCode = partnerCode != null ? partnerCode : "";
            MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(partnerCode);
            ViewBag.PartnerName = checkPartner?.PartnerName != null ? checkPartner.PartnerName : "";
            return PartialView("PopupRegisterItemsPartner");
        }

        [HttpGet]
        public object ShowPopupItemsPartnerImportExcel(string partnerCode, string threadId)
        {
            ViewBag.Thread = threadId;
            ViewBag.PartnerCode = partnerCode != null ? partnerCode : "";
            MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(partnerCode);
            ViewBag.PartnerName = checkPartner?.PartnerName != null ? checkPartner.PartnerName : "";
            return PartialView("PopupRegisterItemsPartnerImportExcel");
        }
        [HttpGet]
        public object getListItemByPartner(DataSourceLoadOptions loadOptions, string partnerCode, string flag, string itemName, string itemCode)
        {
            List<MES_ItemPartner> listItemSupplied = new List<MES_ItemPartner>();
            //if (flag != null && flag.Equals("getListItems"))
            //{
            //    if (partnerCode != null)
            //    {
            //        listItemSupplied = mESItemPartnerService.GetListItemSupply(partnerCode);
            //        int no = 1;
            //        foreach (var item in listItemSupplied)
            //        {
            //            item.No = no++;
            //        }
            //    }
            //}
            //else if (flag != null && flag.Equals("search"))
            //{
            //    if (partnerCode != null)
            //    {
            //        listItemSupplied = mESItemPartnerService.SearchItemByPartner(partnerCode, itemName, itemCode);
            //        int no = 1;
            //        foreach (var item in listItemSupplied)
            //        {
            //            item.No = no++;
            //        }
            //    }
            //}

            listItemSupplied = mESItemPartnerService.SearchItemByPartner(partnerCode, itemName, itemCode);

            return DataSourceLoader.Load(listItemSupplied, loadOptions);
        }
        [HttpGet]
        public object getItemByParam(string partnerCode, string itemCode)
        {
            var listItemSupplied = mESItemPartnerService.GetItemPartnerByParams(partnerCode, itemCode);
            return listItemSupplied;
        }

        public object ShowCreatePopUp()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            return PartialView("PopupCreateItemPO");
        }
        [HttpGet]
        public object getListMonetaryUnit()
        {
            var listMonetaryUnit = mESItemPartnerService.getListMonetaryUnit();
            return listMonetaryUnit;
        }
        [HttpGet]
        public object ShowPopupProjectCode(string threadId)
        {
            //ViewBag.Thread = Guid.NewGuid().ToString("N"); //viewbag ko dc trung
            ViewBag.Thread = threadId;
            var listSaleProject = _mesSaleProjectService.GetListProjectCodeByStatus(); //sale project with status request
            ViewBag.ProjectCode = listSaleProject != null ? listSaleProject[0].ProjectCode : "";
            return PartialView("PopupShowProjectCode", listSaleProject);
        }
        [HttpGet]
        public IActionResult ShowPopupPartner(string threadId, string projectCode)
        {
            ViewBag.Thread = threadId;
            var listPartner = new List<MES_Partner>();
            var distinctPartner = new List<MES_Partner>();
            if (!string.IsNullOrEmpty(projectCode))
            {
                //GetListPartner
                listPartner = _partnerService.getListPartner_ByProjectCode(projectCode);
                foreach (var item in listPartner)
                {
                    if (!distinctPartner.Any(x => x.PartnerCode == item.PartnerCode))
                    {
                        distinctPartner.Add(item);
                    }
                }
            }
            else
            {
                //GetListPartnerNoParams
                distinctPartner = _partnerService.GetAllPartner(); //tra ra full partner 
            }
            int no = 1;
            foreach (var item in distinctPartner)
            {
                item.NO = no++;
            }
            return PartialView("PopupShowPartner", distinctPartner);
        }
        [HttpGet]
        public object SearchPartner(DataSourceLoadOptions loadOptions, string partnerCode, string partnerName, string projectCode)
        {
            var listPartner = new List<MES_Partner>();
            var listDistinct = new List<MES_Partner>();
            // tai vi query ra nhieu Partner do co Join voi nhieu item -> nen dictinct chu yeu lay ra 
            // 1 thang partner cho ng ta chon thoi
            if (!string.IsNullOrEmpty(projectCode))
            {
                listPartner = _partnerService.SearchPartnerByProjectCode(partnerCode, partnerName, projectCode);
            }
            else
            {   //search partner theo project code
                listPartner = _partnerService.SearchPartner(partnerCode, partnerName);
            }
            foreach (var item in listPartner)
            {
                if (!listDistinct.Any(x => x.PartnerCode == item.PartnerCode))
                {
                    listDistinct.Add(item);
                }
            }
            return DataSourceLoader.Load(listDistinct, loadOptions);
        }
        [HttpGet]
        public object GetItems()
        {
            var listSaleProject = _mesSaleProjectService.GetListProjectCodeByStatus(); //sale project with status request
            ViewBag.ProjectCode = listSaleProject != null ? listSaleProject[0].ProjectCode : "";
            return PartialView("PopupShowProjectCode", listSaleProject);
        }
        public object GetItemClass(DataSourceLoadOptions loadOptions)
        {
            var data = pORequestService.GetListItemClassCode();
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion

        #region OverSea
        public IActionResult GetListComCodeDTL(DataSourceLoadOptions loadOptions, string groupCD)
        {
            var data = mESComCodeService.GetListComCodeDTL(groupCD);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpPost]
        public IActionResult POClose(string PONumber)
        {
            var result = pORequestService.POClose(PONumber);
            return Json(result);
        }
        public IActionResult CheckStatusPOClose(string PONumber)
        {
            var result = pORequestService.CheckStatusPOClose(PONumber);
            return Json(result);
        }
        #endregion

        public IActionResult GetPOStatus(string PONumber)
        {
            var result = pORequestService.GetPOStatus(PONumber);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetListPartCombobox()
        {
            var data = pORequestService.GetListPartnerCombobox();
            return Json(data);
        }

        [HttpPost]
        public IActionResult CoppyPO(string PONumber)
        {
            var result = pORequestService.CoppyPO(PONumber, CurrentUser.UserID);
            return Json(result);
        }

        [HttpPut]
        public IActionResult UpdateRemakAfterConfirmed(string PONumber, string Remark)
        {
            var result = pORequestService.UpdateRemakAfterConfirmed(PONumber, Remark);
            return Json(result);
        }

        [HttpPost]
        public object ShowPopupSendEmail(string threadId, MES_PORequest PORequest)
        {
            ViewBag.Thread = threadId;
            return PartialView("PopupSendMail", PORequest);
        }

        [HttpPost]
        public object ShowPopupReSendEmail(string threadId, MES_PORequest PORequest)
        {
            ViewBag.Thread = threadId;
            return PartialView("PopupReSendMail", PORequest);
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailPORequest(string PONumber, string EmailTo, string UserPONumber)
        {
            try
            {
                var objects = new EMailResponse();
                var listData = new List<EMailResponse>();
                var result = new Result();
                var emailAccount = new MailAddress(_config.GetValue<string>("MailSettings:EmailAccount"));
                var url = _config.GetValue<string>("MailSettings:LinkUrl");
                List<string> listEmailTo = JsonConvert.DeserializeObject<List<string>>(EmailTo);
                var rs = "";
                foreach (var item in listEmailTo)
                {
                    rs += item + ", ";
                }

                var listPurcharDetail = pORequestService.GetListPurcharDetail(PONumber);
                objects.listItem = listPurcharDetail;
                objects.EmailTo = rs;
                objects.EmailFrom = emailAccount.Address;
                objects.PoNumber = PONumber;
                objects.UserPONumber = UserPONumber;

                objects.Url = url;

                var emailBody = await _razorViewRenderer.RenderViewToStringAsync<EMailResponse>("~/EmailTemplate/EmailTemplate.cshtml", objects).ConfigureAwait(true);
                result = await _sendMailServices.SendEmailTo(listEmailTo, emailBody, "Purchar Order", true).ConfigureAwait(true);
                return Json(result);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult UploadFilePopup(IFormFile myFile, string chunkMetadata)
        {
            try
            {
                Type myType = Type.GetType("Modules.Pleiger.CommonModels.MES_ItemPO, Modules.Pleiger.CommonModels");
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
        [HttpGet]
        public object InsertDataFromExcelItemPartList(DataSourceLoadOptions loadOptions,string fileLoc,string partnercode)
        {

            string SPName = "SP_MES_ITEM_PO_IMPORT_EXCEL";
            var data = _uploadFileWithTemplateService.Getdata_Model_Excel(fileLoc, SPName, CurrentUser.UserID, "PODetail", partnercode);
            return DataSourceLoader.Load(data, loadOptions);

        }
    }
}
