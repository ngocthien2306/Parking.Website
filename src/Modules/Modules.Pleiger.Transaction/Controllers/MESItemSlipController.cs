using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Transaction.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.Design;
using InfrastructureCore;
using InfrastructureCore.RapidReport;
using System.Data;
using InfrastructureCore.Utils;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-07-28
/// Item Slip Management
/// </summary>
namespace Modules.Pleiger.Transaction.Controllers
{
    public class MESItemSlipController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESItemSlipService _mesItemSlipService;
        private readonly IAccessMenuService _accessMenuService;

            
        public MESItemSlipController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IMESItemSlipService mesItemSlipService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesItemSlipService = mesItemSlipService;
            _accessMenuService = accessMenuService;

        }

        /// <summary>
        /// 출고 nhap hang Partner da lam xong cho Pleiger
        /// </summary>
        /// <returns></returns>
        public IActionResult ReleaseItemSlip()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            ViewBag.CREATE_YN = false;
            ViewBag.SEARCH_YN = false;
            ViewBag.DELETE_YN = false;
            ViewBag.SAVE_YN   = false;

            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);

            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
            }

            //var curUrlTemp = (Request.Path.Value + Request.QueryString);
            //var curUrl = URLRequest.URLSubstring(curUrlTemp);
            //var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            //ViewBag.MenuId = curMenu.MenuID;
            //ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        /// <summary>
        /// 출하관리 xuat hang Pleiger da lam xong cho Customer
        /// </summary>
        /// <returns></returns>
        public IActionResult DeliveryItemSlip()
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

        #region Get Data ItemSlip Master PO  - release item from Partner to Pleiger
        [HttpGet]
        public object GetListMESItemSlipMasterForRelease(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                            string userProjectNo, string userPoNumber, string goodReceipt)
        {
            var data = _mesItemSlipService.GetListMESItemSlipMaster(startDate, endDate, status, userProjectNo, userPoNumber, goodReceipt);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetListMESItemSlipDetailForRelease(DataSourceLoadOptions loadOptions, string slipNumber, string poNumber)
        {
            var data = _mesItemSlipService.GetListMESItemSlipDetail(slipNumber, poNumber);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetListMESItemSlipDetailForReleasePartner(DataSourceLoadOptions loadOptions, string slipNumber, string poNumber)
        {
            var data = _mesItemSlipService.GetListMESItemSlipDetailForReleasePartner(slipNumber, poNumber);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        
        [HttpGet]
        public object GetItemSlipDetailInPopup(DataSourceLoadOptions loadOptions, string poNumber)
        { 
            var data = _mesItemSlipService.GetItemSlipDetailInPopup(poNumber);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        // Add by Quan
        public object GetItemSlipDetailInPopupSearchPO(DataSourceLoadOptions loadOptions, string poNumber)
        {
            var data = _mesItemSlipService.GetItemSlipDetailInPopupSearchPO(poNumber);
            if(data.Count>0)
            {
                ViewBag.StatusCode = data[0].ItemStatus;
            }
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetPONumberForRelease(DataSourceLoadOptions loadOptions)
        {
            List<Combobox> data = new List<Combobox>();
            data = _mesItemSlipService.GetPONumberForRelease();
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetPOAllNumberForRelease(DataSourceLoadOptions loadOptions)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetPOAllNumberForRelease();
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetPOAllNumberSearch(DataSourceLoadOptions loadOptions, string UserPONumber, string PartnerName)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetPOAllNumberSearch(UserPONumber, PartnerName);
            //add total received qty , poqty rồi
            //nếu 2 cái bằng nhau thì filter lại cái list ko cho nó hien ra
            foreach (var item in data.ToList())
            {
                if (item.TotalReceivedQty == item.TotalPOQty)
                {
                    data.Remove(item);
                }
            }
            int no = 1;
            data.ForEach(x =>
            {
                x.No = no++;
            });

            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object CreateGridItemSlipDtlByPONumber(DataSourceLoadOptions loadOptions, string poNumber)
        {
            var data = _mesItemSlipService.CreateGridItemSlipDtlByPONumber(poNumber);
            //bao add
            foreach (var item in data.ToList())
            {
                item.Qty = item.POQty; //set default value receiveQty is POQty
                if (item.POQty == 0)
                {
                    data.Remove(item);
                }
            }
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object CreateGridItemSlipDtlByPONumberPartner(DataSourceLoadOptions loadOptions, string poNumber)
        {
            var data = _mesItemSlipService.CreateGridItemSlipDtlByPONumberPartner(poNumber);
            //bao add
            foreach (var item in data.ToList())
            {
                item.Qty = item.POQty; //set default value receiveQty is POQty
                if (item.POQty == 0)
                {
                    data.Remove(item);
                }
            }
            //
            return DataSourceLoader.Load(data, loadOptions);
        }
        
        [HttpGet]
        public IActionResult GetDataReferByPONumberForRelease(string PONumber)
        {
            var data = _mesItemSlipService.GetDataReferByPONumberForRelease(PONumber);
            //return Content(JsonConvert.SerializeObject(data), "application/json");
            return Json(new { data });
        }

        [HttpGet]
        public IActionResult GetItemSlipMasterKey()
        {
            var data = _mesItemSlipService.GetItemSlipMasterKey();
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        #endregion

        #region "Insert - Update - Delete - release item from Partner to Pleiger"

        // Save Data ItemSlipMaster
        [HttpPost]
        public IActionResult SaveDataMaterialInStock(string flag, string itemSlipMaster, string itemSlipDetail)
        {
            MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(itemSlipMaster);
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);
            var result = _mesItemSlipService.SaveDataMaterialInStock(flag, slipMaster, slipDetails, CurrentUser.UserID);
            return Json(result);
        }

        // Delete Data ItemSlipMaster
        [HttpPost]
        public IActionResult DeleteItemSlip(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl)
        {
            //MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(data);
            var result = _mesItemSlipService.DeleteItemSlip(dataMst, dataDtl, CurrentUser.UserID);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DeleteItemSlipDetail(MES_ItemSlipDetail dataDtl)
        {
            var result = _mesItemSlipService.DeleteItemSlipDetail(dataDtl);

            return Json(result);
        }

        [HttpPost]
        public  IActionResult UpdateSlipDate(string slipNumber, DateTime? slipDate)
        {

            var result = _mesItemSlipService.UpdateSlipDate(slipNumber, slipDate);

            return Json(result);
        }
        #endregion

        #region  Get Data ItemSlip Project have production complete  - prepare item deliver from Pleiger to Partner/Customer
        [HttpGet]
        public object GetListProjectPrepareDelivery(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                            string projectNo, string itemCodeSearch, string itemNameSearch, string prodcnCodeSearch, string projectOrderType
                            , string saleOrderProjectName, string userProjectCode)
        {
            var data = _mesItemSlipService.GetListProjectPrepareDelivery(startDate, endDate, status, projectNo, itemCodeSearch, itemNameSearch, prodcnCodeSearch, projectOrderType, saleOrderProjectName, userProjectCode);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object ProjectPrepareDeliveryDataGridMasterDetailView(DataSourceLoadOptions loadOptions, string ProjectCode)
        {
            var data = _mesItemSlipService.ProjectPrepareDeliveryDataGridMasterDetailView(ProjectCode);
            return DataSourceLoader.Load(data, loadOptions);
        }

        // Get list customer
        [HttpGet]
        public object GetCustomerPartnerCode(DataSourceLoadOptions loadOptions, string projectNo)
        {
            var data = _mesItemSlipService.GetCustomerPartnerCode(projectNo);
            return DataSourceLoader.Load(data, loadOptions);
        }

        // Get list pleiger warehouse code contain itemcode
        [HttpGet]
        public object GetWarehousePGItem(DataSourceLoadOptions loadOptions, string ItemCode)
        {
            var data = _mesItemSlipService.GetWarehousePGItem(ItemCode);
            return DataSourceLoader.Load(data, loadOptions);
        }
        // Quan add
        // Get list pleiger warehouse code contain itemcode
        [HttpGet]
        public object GetQtyWarehousePGItem(DataSourceLoadOptions loadOptions, string ItemCode)
        {
            var data = _mesItemSlipService.GetQtyWarehousePGItem(ItemCode);
            return DataSourceLoader.Load(data, loadOptions);
        }
        // Get list partner warehouse of each partner code
        [HttpGet]
        public object GetWarehouseOfPartner(DataSourceLoadOptions loadOptions)
        {
            //string PartnerCode = loadOptions.Filter != null ? (string)loadOptions.Filter[2] : "";
            var data = _mesItemSlipService.GetWarehouseOfPartner();
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetListSlipDelivery(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var data = _mesItemSlipService.GetListSlipDelivery(projectCode);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }

        #endregion

        #region "Insert - Update - Delete - item deliver from Pleiger to Partner"
        // Save Data ItemSlipMaster
        [HttpPost]
        public IActionResult SaveDataDeliveryOutStock(string flag, string itemSlipMasterDtl)
        {
            List<MES_ItemSlipDelivery> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDelivery>>(itemSlipMasterDtl);
            var result = _mesItemSlipService.SaveDataDeliveryOutStock(flag, slipDetails, CurrentUser.UserID);
            return Json(result);
        }


        [HttpPost]
        public IActionResult DeleteItemSlipDelivery(List<MES_ItemSlipDelivery> data)
        {
            //MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(data);
            var result = _mesItemSlipService.DeleteItemSlipDelivery(data, CurrentUser.UserID);
            return Json(result);
        }
        #endregion

        #region Moving Stock in Pleiger
        /// <summary>
        /// Return view
        /// </summary>
        /// <returns></returns>
        public IActionResult StockMovingItemSlip()
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

        [HttpGet]
        public IActionResult StockMovingItemSlipGetItem(string WareHouseCode, string vbThread)
        //public IActionResult ProductionRequestPopup()
        {
            ViewBag.OldThread = vbThread;
            ViewBag.WareHouseCode = WareHouseCode;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MES_ItemSlipDetail();

            return PartialView("StockMovingItemSlipGetItem", model);
        }

        [HttpGet]
        public object GetListMovingStockItem(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                        string fromWH, string toWH)
        {
            var data = _mesItemSlipService.GetListMovingStockItem(startDate, endDate, status, fromWH, toWH);
            return DataSourceLoader.Load(data, loadOptions);
        }


        [HttpGet]
        public object GetListMovingStockItemDetail(DataSourceLoadOptions loadOptions, string slipNumber)
        {
            var data = _mesItemSlipService.GetListMovingStockItemDetail(slipNumber);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public IActionResult PopupSearchReturnPO(string Index)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = Index;
            return View();
        }
        [HttpGet]
        public IActionResult PopupSearchPO(string Index)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = Index;
            return View();
        }

        public IActionResult PopupScanner(string Index)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = Index;
            return View();
        }
        [HttpGet]
        public IActionResult CheckQtyInputItemSlipDetail(string itemSlipDetail)
        {
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);

            if (slipDetails.Count == 0)
            {
                return Json("1");
            }
            else
            {
                var ResultCheck = slipDetails.Where(x => x.Qty > x.RealQty).FirstOrDefault();
                var ResultCheckRealQty = slipDetails.Where(x => x.Qty == 0 || x.RealQty == null || x.RealQty == 0).FirstOrDefault();

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
        // Save Data ItemSlipMaster
        [HttpPost]
        public IActionResult SaveMovingStockItem(string flag, string itemSlipMaster, string itemSlipDetail)
        {
            MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(itemSlipMaster);
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);
            var result = _mesItemSlipService.SaveMovingStockItem(flag, slipMaster, slipDetails, CurrentUser.UserID);
            return Json(result);
        }

        // Delete Data ItemSlipMaster
        [HttpPost]
        public IActionResult DeleteMovingStockItem(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl)
        {
            //MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(data);
            var result = _mesItemSlipService.DeleteMovingStockItem(dataMst, dataDtl, CurrentUser.UserID);
            return Json(result);
        }

        // Project Status 
        //PJST04 생산진행    Product Progress
        //PJST05 생산완료    Product Completed
        [HttpGet]
        public IActionResult GetProjectNameInProduction(DataSourceLoadOptions loadOptions)
        {
            var data = _mesItemSlipService.GetProjectNameInProduction();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        #endregion

        #region Goods Return PO
        public IActionResult GoodsReturnPO()
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

        [HttpGet]
        public object GetItemSlipMasterGoodsReturnPO(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                           string partnerCode, string userProjectCode, string userPoNumber)
        {
            var data = _mesItemSlipService.GetItemSlipMasterGoodsReturnPO(startDate, endDate, status, partnerCode, userProjectCode, userPoNumber);
            return DataSourceLoader.Load(data, loadOptions);
        }


        [HttpGet]
        public object GetPONumberHaveReceipt(DataSourceLoadOptions loadOptions)
        {
            List<Combobox> data = new List<Combobox>();
            data = _mesItemSlipService.GetPONumberHaveReceipt();
            return DataSourceLoader.Load(data, loadOptions);
        }
        // Quan add 2020/09/29
        // GetPONumberHaveReceipt Popup Search

        [HttpGet]
        public object GetPONumberHaveReceiptSearch(DataSourceLoadOptions loadOptions, string UserPONumber, string PartnerName)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetPONumberHaveReceiptSearch(UserPONumber, PartnerName);
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object CreateGridItemSlipDtlByPONumberInGoodsReturn(DataSourceLoadOptions loadOptions, string poNumber)
        {
            var data = _mesItemSlipService.CreateGridItemSlipDtlByPONumberInGoodsReturn(poNumber);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpPost]
        public IActionResult SaveDataGoodsReturn(string flag, string itemSlipMaster, string itemSlipDetail)
        {
            MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(itemSlipMaster);
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);
            var result = _mesItemSlipService.SaveDataGoodsReturn(flag, slipMaster, slipDetails, CurrentUser.UserID);
            return Json(result);
        }

        [HttpGet]
        public object GetListMESItemSlipDetailGoodsReturn(DataSourceLoadOptions loadOptions, string slipNumber, string poNumber)
        {
            var data = _mesItemSlipService.GetListMESItemSlipDetailGoodsReturn(slipNumber, poNumber);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetListMESItemSlipDetailGoodsReturnDelivery(DataSourceLoadOptions loadOptions, string slipNumber, string poNumber,string slipNumberGoodReceipt)
        {
            var data = _mesItemSlipService.GetListMESItemSlipDetailGoodsReturnDelivery(slipNumber, poNumber, slipNumberGoodReceipt);
            loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "Qty", SummaryType = "sum" }
                };
            return DataSourceLoader.Load(data, loadOptions);
        }
        

        [HttpPost]
        public IActionResult DeleteDataGoodsReturn(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl)
        {
            var result = _mesItemSlipService.DeleteDataGoodsReturn(dataMst, dataDtl, CurrentUser.UserID);
            return Json(result);
        }
        #endregion

        #region Goods Return Delivery

        #region Show View
        public IActionResult GoodsReturnDelivery()
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
        #endregion

        #region Get Data
        [HttpGet]
        public object GetItemSlipMasterGoodsReturnDelivery(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                           string partnerCode, string userProjectCode)
        {
            var data = _mesItemSlipService.GetItemSlipMasterGoodsReturnDelivery(startDate, endDate, status, partnerCode, userProjectCode);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public IActionResult PopupSearchReturnDelivery(string Index)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = Index;
            return View();
        }
        #endregion

        #region Get data
        [HttpGet]
        public object GetProjectHaveGoodsIssuesSearch(DataSourceLoadOptions loadOptions, string userProjectCode, string partnerName)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetProjectHaveGoodsIssuesSearch(userProjectCode, partnerName);
            return DataSourceLoader.Load(data, loadOptions);
        }
        // Quan add 2021-01-27
        [HttpGet]
        public object GetListGoodsDeliveries(DataSourceLoadOptions loadOptions, string userProjectCode, string partnerName,string ProjectCode)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetListGoodsDeliveries(userProjectCode, partnerName, ProjectCode);
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object CreateGridItemSlipDtlByProjectInGoodsReturnDelivery(DataSourceLoadOptions loadOptions, string projectCode,string slipNumber)
        {
            var data = _mesItemSlipService.CreateGridItemSlipDtlByProjectInGoodsReturnDelivery(projectCode, slipNumber);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion

        #region CRUD data
        [HttpPost]
        public IActionResult SaveDataGoodsReturnDelivery(string flag, string itemSlipMaster, string itemSlipDetail, string transSlipNumber)
        {
            MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(itemSlipMaster);
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);
            var result = _mesItemSlipService.SaveDataGoodsReturnDelivery(flag, slipMaster, slipDetails, CurrentUser.UserID, transSlipNumber);
            return Json(result);
        }
        #endregion

        #endregion

        [HttpGet]
        public object getStockQtyByItemCode(DataSourceLoadOptions loadOptions, string ItemCode)
        {
            var data = _mesItemSlipService.getStockQtyByItemCode(ItemCode);         
            return DataSourceLoader.Load(data, loadOptions);
        }
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";
        private const string EXCEL_FILE_NAME = "PORequest";

        public ActionResult GetDetailPODelivery(string poNumber, string Model)
        {
            decimal? price = 0;
            decimal? vat = 0;
            List<MES_ItemSlipMaster> item = JsonConvert.DeserializeObject<List<MES_ItemSlipMaster>>(Model);
            var PDFDetail = _mesItemSlipService.GetListMESItemSlipDetail(item[0].SlipNumber, item[0].RelNumber);
            PDFDetail[0].PartnerName = item[0].PartnerName;
            PDFDetail[0].QrCode = item[0].SlipNumber;
            PDFDetail[0].RegistNumber = item[0].RegistNumber;
            PDFDetail[0].UserPONumber= item[0].UserPONumber;
            PDFDetail[0].PartnerAddress = item[0].PartnerAddress;
            foreach (var data in PDFDetail)
            {
                data.Qty = (int) data.Qty;
                data.Total = data.Cost * data.Qty;
                price += data.Total;
            }
            vat = price * (decimal)0.1;
            PDFDetail[0].TotalPrice = price;
            PDFDetail[0].Vat = vat;
            PDFDetail[0].SumVat = vat + price;
            PDFDetail[0].DeliveryDate = item[0].PartnerDeliveryDate;
            PDFDetail[0].Ceo = item[0].Ceo;
            string partnerCountry = "CTTP01";

            if (PDFDetail.Count > 0)
            {

                //string PDFFileName = $"{EXCEL_FILE_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.rrpt";
                Result result = new Result();
                RapidReportModule rapid = new RapidReportModule();
                string ouputfilePath = "";
                string ouputfilePath1 = "";

                DataTable dataTable = ExcelExport.ConvertToDataTable(PDFDetail);
                try
                {
                    ouputfilePath = partnerCountry == "CTTP01" ? rapid.Run(dataTable, "PFE_Transaction_en", "PFE_Transaction", "En") : rapid.Run(dataTable, "PurchaseOrder_en", "PFE_Transaction", "En");
                    ouputfilePath1 = partnerCountry == "CTTP01" ? rapid.Run(dataTable, "PFE_Transaction_en1", "PFE_Transaction", "En") : rapid.Run(dataTable, "PurchaseOrder_en", "PFE_Transaction", "En");

                }

                catch (Exception ex)
                {
                    result.Message = ex.Message.ToString();
                    result.Success = false;
                    return Json(new { Result = result });
                }

                var memory = new MemoryStream();
                var memory1 = new MemoryStream();
                using (var stream = new FileStream(ouputfilePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                using (var stream1 = new FileStream(ouputfilePath1, FileMode.Open))
                {
                    stream1.CopyTo(memory1);
                }
                memory.Position = 0;
                var file = File(memory, ExcelExport.GetContentType(ouputfilePath), ouputfilePath.Remove(0, ouputfilePath.LastIndexOf("\\") + 1));
                var file1 = File(memory1, ExcelExport.GetContentType(ouputfilePath1), ouputfilePath1.Remove(0, ouputfilePath1.LastIndexOf("\\") + 1));

                result.Success = true;
                return Json(new { Result = result, downloadExcelPath = ouputfilePath, downloadExcelPath1 = ouputfilePath1, fileName = file.FileDownloadName });

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

        // Slide 40    New menu, This menu is for partner use only.
        // Purchase Order Delivery
        public IActionResult PartnerPODelivery()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            ViewBag.CREATE_YN = false;
            ViewBag.SEARCH_YN = false;
            ViewBag.DELETE_YN = false;
            ViewBag.SAVE_YN = false;

            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);

            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
                ViewBag.SAVE_YN   = ListButtonPermissionByUser[0].SAVE_YN;
            }
            return View();
        }


        [HttpGet]
        public object GetListPODeliverybyPartner(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,string userProjectNo, string userPoNumber)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            if (CurrentUser.UserType == "G000C005" || CurrentUser.UserType == "G000C007")
            {
                data = _mesItemSlipService.GetListPODeliverybyPartner(startDate, endDate, status, userProjectNo, userPoNumber, CurrentUser.UserCode);
            }
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public IActionResult PopupSearchPOPartner(string Index)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = Index;
            return View();
        }
        [HttpGet]
        public object GetPOAllNumberSearchByPartner(DataSourceLoadOptions loadOptions, string UserPONumber)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            string PartnerCode = null;
            if(CurrentUser.SystemUserType == "G000C005" || CurrentUser.SystemUserType == "G000C007")
            {
                PartnerCode = CurrentUser.UserCode;
            }

            data = _mesItemSlipService.GetPOAllNumberSearchByPartner(UserPONumber, PartnerCode);
            //add total received qty , poqty rồi
            //nếu 2 cái bằng nhau thì filter lại cái list ko cho nó hien ra
            foreach (var item in data.ToList())
            {
                if (item.TotalReceivedQty == item.TotalPOQty)
                {
                    data.Remove(item);
                }
            }
            int no = 1;
            data.ForEach(x =>
            {
                x.No = no++;
            });

            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetListMESItemSlipMasterForReleasePartner(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status, string userProjectNo, string userPoNumber)
        {
            string loginUser = null;
            if(CurrentUser.UserType == "G000C005" || CurrentUser.UserType == "G000C007")
            {
                loginUser = CurrentUser.UserCode;
            }    
            var data = _mesItemSlipService.GetListMESItemSlipMasterForReleasePartner(startDate, endDate, status, userProjectNo, userPoNumber, loginUser);
            return DataSourceLoader.Load(data, loadOptions);
        }


        // Save Data ItemSlipMaster and ItemSlipMasterDetail
        //create a temporary delivery note
        [HttpPost]
        public IActionResult SavePartnerPODelivery(string flag, string itemSlipMaster, string itemSlipDetail)
        {
            MES_ItemSlipMaster slipMaster = JsonConvert.DeserializeObject<MES_ItemSlipMaster>(itemSlipMaster);
            List<MES_ItemSlipDetail> slipDetails = JsonConvert.DeserializeObject<List<MES_ItemSlipDetail>>(itemSlipDetail);
            //var result = _mesItemSlipService.SaveDataMaterialInStock(flag, slipMaster, slipDetails, CurrentUser.UserID);
            var result = _mesItemSlipService.SavePartnerPODelivery(flag, slipMaster, slipDetails, CurrentUser.UserID);
            
            return Json(result);
        }

        // get detail by list of project code (json)
        [HttpPost]
        public IActionResult getDetailByProjectCodeList(string listProjectCode)
        {
            var result = _mesItemSlipService.getDetailByProjectCodeList(listProjectCode);
            return Json(result);
        }
    }
}
