using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-07-28
/// Item Slip Management
/// </summary>
namespace Modules.Pleiger.Controllers
{
    public class MESItemSlipController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESItemSlipService _mesItemSlipService;

        public MESItemSlipController(IHttpContextAccessor contextAccessor, IMESItemSlipService mesItemSlipService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesItemSlipService = mesItemSlipService;
        }

        /// <summary>
        /// 출고 nhap hang Partner da lam xong cho Pleiger
        /// </summary>
        /// <returns></returns>
        public IActionResult ReleaseItemSlip()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
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
                            string userProjectNo, string userPoNumber)
        {
            var data = _mesItemSlipService.GetListMESItemSlipMaster(startDate, endDate, status, userProjectNo, userPoNumber);
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
        public object GetPOAllNumberSearch(DataSourceLoadOptions loadOptions,string UserPONumber,string PartnerName)
        {
            List<MES_ItemSlipMaster> data = new List<MES_ItemSlipMaster>();
            data = _mesItemSlipService.GetPOAllNumberSearch(UserPONumber, PartnerName);
            //add total received qty , poqty rồi
            //nếu 2 cái bằng nhau thì filter lại cái list ko cho nó hien ra
            foreach(var item in data.ToList()) 
            {
                if(item.TotalReceivedQty == item.TotalPOQty)
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
        #endregion

        #region  Get Data ItemSlip Project have production complete  - prepare item deliver from Pleiger to Partner/Customer
        [HttpGet]
        public object GetListProjectPrepareDelivery(DataSourceLoadOptions loadOptions, string startDate, string endDate, string status,
                            string projectNo, string itemCodeSearch, string itemNameSearch, string prodcnCodeSearch)
        {
            var data = _mesItemSlipService.GetListProjectPrepareDelivery(startDate, endDate, status, projectNo, itemCodeSearch, itemNameSearch, prodcnCodeSearch);
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

                if (ResultCheck != null || ResultCheckRealQty!=null)
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
                           string partnerCode)
        {
            var data = _mesItemSlipService.GetItemSlipMasterGoodsReturnPO(startDate, endDate, status, partnerCode);
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

        [HttpPost]
        public IActionResult DeleteDataGoodsReturn(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl)
        {
            var result = _mesItemSlipService.DeleteDataGoodsReturn(dataMst, dataDtl, CurrentUser.UserID);
            return Json(result);
        }
        #endregion
    }
}
