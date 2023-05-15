using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using System.IO;
using InfrastructureCore.Utils;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using Modules.Pleiger.Services.ServiceImp;
using Modules.Admin.Services.IService;
using Modules.Common.Models;

namespace Modules.Pleiger.Controllers
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
        private readonly IAccessMenuService accessMenuService;
        private readonly IUserService _userService;


        public MESPORequestController(IHttpContextAccessor contextAccessor,
            IAccessMenuService accessMenuService,
            IPORequestService pORequestService,
            IMESComCodeService mESComCodeService,
            IMESItemPartnerService mESItemPartnerService,
            IPurchaseService purchaseService,
            IMESPartnerService partnerService,
            IMESSaleProjectService mesSaleProjectService,
            IUserService userService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.pORequestService = pORequestService;
            this.mESComCodeService = mESComCodeService;
            this.mESItemPartnerService = mESItemPartnerService;
            this._purchaseService = purchaseService;
            this._partnerService = partnerService;
            this._mesSaleProjectService = mesSaleProjectService;
            this.accessMenuService = accessMenuService;
            this._userService = userService;


        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.MenuIDParent = CurrentMenu.MenuID;
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            if(CurrentUser == null)
            {
                return NotFound();
            }
            var SelectUserPermissionAccessMenu = accessMenuService.SelectUserPermissionAccessMenu(CurrentMenu.MenuID, CurrentUser.UserID);
            if(SelectUserPermissionAccessMenu.Count > 0)
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


            //case check must be partner code and country to show grid
            //MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(checkUserLogin.UserCode);
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
        public IActionResult SearchListPORequest(string projectCode, string poNumber,string userPONumber,string userProjectCode, 
            string requestDateFrom, string requestDateTo ,string poStatus)
        {
            string partnerCode = "";
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID); //
           
            //nếu employee null tức là nó là partner chính => lấy userlogin. cái user code lun
            //nếu employee != null tức là nó là nhan vien của partner đó => lấy employ.partner
            if(checkUserLogin.SystemUserType.Equals("G000C005") || checkUserLogin.SystemUserType.Equals("G000C007") || checkUserLogin.SystemUserType.Equals("G000C006")) //them 007 
            {
                var checkUserEmployee = new CHECKRESULT();
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
                var data = pORequestService.SearchListPORequest(projectCode, poNumber, userPONumber, userProjectCode, requestDateFrom, requestDateTo, partnerCode,poStatus);
                return Content(JsonConvert.SerializeObject(data),"application/json");
            }
            // Pleiger
            else
            {
                //case nếu user đó ko nằm rong employee .mà nằm trong SyUser
                var data = pORequestService.SearchListPORequest(projectCode, poNumber, userPONumber, userProjectCode, requestDateFrom, requestDateTo, "", poStatus);
                return Content(JsonConvert.SerializeObject(data), "application/json");
            }
        }

        // Show detail PO Request
        [HttpGet]
        public IActionResult ShowDetailPORequest(string projectCode, string poNumber,string partnerCode,int menuIDParent)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.menuIDParent = menuIDParent;
            var SelectUserPermissionAccessMenu = accessMenuService.SelectUserPermissionAccessMenu(menuIDParent, CurrentUser.UserID);
            if(SelectUserPermissionAccessMenu.Count > 0)
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
                foreach(var item in listItemPORequest)
                {
                    ViewBag.MonetaryDisplayInGrid = item.MonetaryUnit;
                    ViewBag.PrintingStatus = item.PrintingStatus;
                    break;
                }
                try
                {
                    model.ListItemPORequest = listItemPORequest;
                }catch(Exception ex)
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
                object listPartnerDTL =  GetListPartner(projectCode); //when get detail of PO => send a list partner to detail screen
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
            }
         
            return PartialView("DetailNewPORequest", model);
        }
        // Get list Lead Time Type
        [HttpGet]
        public IActionResult GetListLeadTimeType()
        {
            var data = mESComCodeService.GetListComCodeDTL("LDTM00");
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        public List<MES_SaleProject> SearchProjectCode(string projectCode, string itemCode, string itemName)
        {
            var data = pORequestService.SearchProjectCodeByParams(projectCode, itemCode, itemName);
            return data;
        }
        [HttpGet]
        public List<MES_SaleProject> GetProjectCodeByStatus()
        {
            var data = pORequestService.GetProjectCodeByStatus();
            return data;
        }
        #endregion

        #region "Insert - Update - Delete"

        [HttpPost]
        public IActionResult SaveDataPORequest(string flag , MES_PORequest PORequest )
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
            var result = pORequestService.SavePO(flag,PORequest,listDetails, CurrentUser.UserID);
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
        public IActionResult CheckPartnerOverSea(string partnerCode,string threadId)
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
        public IActionResult GetOverSeaDetail(string projectCode , string poNumber, string partnerCode, string threadId)       
        {
            var model = new MES_PORequest();
            MES_Partner partner = pORequestService.getPartnerByPartnerCode(partnerCode);
            if (projectCode != null || poNumber != null)
            {
                model = pORequestService.GetPODetail(poNumber);
                var listItemPORequest = pORequestService.GetListItemPORequest( poNumber);
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
            return PartialView("POOverSea",partner);
        }
        //update PO detail partner
        [HttpPost]
        public IActionResult UpdatePODetailpartner(string flag, MES_PORequest PORequest)
        {
            List<MES_ItemPO> listDetails = JsonConvert.DeserializeObject<List<MES_ItemPO>>(PORequest.ListItemPO);
          
            var result = pORequestService.UpdatePODetailPartner(flag, PORequest,listDetails);
            return Json(result);
        }
        // Quan add 2020/09/28
        // Delete POMst
        [HttpPost]
        public IActionResult DeletePOMst(string PONumber)        {
            
            var result = pORequestService.DeletePOMst(PONumber);
            return Json(result);
        }
        
        #endregion

        #region Print PO TO PDF FILE

        [HttpGet]
        public async Task<IActionResult> GetDetailPOReq(string PONumber)
        {
            var data = await pORequestService.CreatePOPrint(PONumber);

            Debug.WriteLine("PVN Test 5 ");
            Debug.WriteLine("GetDetailPOReq Run At " + DateTime.Now);

            var memory = new MemoryStream();
            using (var stream = new FileStream(data, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(true);
            }
            memory.Position = 0;
            var file = await Task.Run(() => File(memory, ExcelExport.GetContentType(data), data.Remove(0, data.LastIndexOf("/") + 1))).ConfigureAwait(true);
            return Json(new { downloadExcelPath = data, fileName = file.FileDownloadName });


            //var contentTYpe = ExcelExport.GetContentType(data);

            //return File(memory, contentTYpe, Path.GetFileName(data));

            //return new FileStreamResult(memory, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/pdf"))
            //{
            //    FileDownloadName = Path.GetFileName(data)
            //};

            //return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        [HttpGet]
        public async Task<ActionResult> Download(string fileLink, string fileName)
        {
            Debug.WriteLine("PVN Test 6 ");
            Debug.WriteLine("Download Run At " + DateTime.Now);
            if (fileName != null)
            {
                //byte[] data = TempData[fileGuid] as byte[];
                //return File(fileName, "application/vnd.ms-excel", fileName);
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
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
        public object GetListPartner(string projectCode)
        {
            var listFullPartner = _partnerService.getListPartner_ByProjectCode(projectCode);
            var distinctSet = new List<MES_ItemPartner>();
            foreach (var item in listFullPartner)
            {
                if (!distinctSet.Any(x => x.PartnerCode == item.PartnerCode))
                {
                    distinctSet.Add(item);
                }
            }
            var listPartner = new MES_ItemPartnerResult()
            {
                listDistinctPartner = distinctSet,
                listFullPartner = listFullPartner
            };
            return listPartner;
        }
        [HttpGet]
        public object GetListPartnerNoParams()
        {
            var listFullPartner = _partnerService.GetAllPartner();
            return listFullPartner;
        }  
        [HttpGet]
        public object ShowPopupItemsPartner(string partnerCode ,string threadId)
        {
            ViewBag.Thread = threadId;
            ViewBag.PartnerCode = partnerCode != null ? partnerCode : "";
            MES_Partner checkPartner = pORequestService.getPartnerByPartnerCode(partnerCode);
            ViewBag.PartnerName = checkPartner?.PartnerName != null ? checkPartner.PartnerName : "";
            return PartialView("PopupRegisterItemsPartner");
        }


        [HttpGet]
        public object getListItemByPartner(DataSourceLoadOptions loadOptions, string partnerCode ,string flag , string itemName,string itemCode)
        {
            List<MES_ItemPartner> listItemSupplied = new List<MES_ItemPartner>();
            if (flag != null && flag.Equals("getListItems"))
            {
                if (partnerCode != null)
                {
                    listItemSupplied = mESItemPartnerService.GetListItemSupply(partnerCode);
                    int no = 1;
                    foreach (var item in listItemSupplied)
                    {
                        item.No = no++;
                    }
                }
            }
            else if (flag != null && flag.Equals("search"))
            {
                if (partnerCode != null)
                {
                    listItemSupplied = mESItemPartnerService.SearchItemByPartner(partnerCode,itemName,itemCode);
                    int no = 1;
                    foreach (var item in listItemSupplied)
                    {
                        item.No = no++;
                    }
                }
            }

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
        #endregion
    }
}
