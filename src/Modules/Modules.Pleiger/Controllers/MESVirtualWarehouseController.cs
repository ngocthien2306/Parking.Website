using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using InfrastructureCore;
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
using System.Data;
using System.IO;
using System.Linq;
using InfrastructureCore.Helpers;
using System.Text;
using InfrastructureCore.Helpers;
using Modules.FileUpload.Models;

namespace Modules.Pleiger.Controllers
{
    public class MESVirtualWarehouseController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESVirtualWarehouseService _mesVirtualWarehouseService;
        private readonly IMESItemService _mESItemService;
        private readonly IItemClassService _itemClassService;
        

        public MESVirtualWarehouseController(IItemClassService itemClassService, IHttpContextAccessor contextAccessor, IMESComCodeService mesComCodeService, IMESVirtualWarehouseService mesVirtualWarehouseService, IMESItemService mESItemService) : base(contextAccessor)
        //public MESVirtualWarehouseController(IHttpContextAccessor contextAccessor, IMESSaleProjectService mesSaleProjectService, IMESComCodeService mesComCodeService, IMESVirtualWarehouseService mesVirtualWarehouseService, IMESItemService mESItemService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mesComCodeService = mesComCodeService;
            _mesVirtualWarehouseService = mesVirtualWarehouseService;
            _mESItemService = mESItemService;
            _itemClassService = itemClassService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            // ViewBag.MenuId = (curMenu) != null ? curMenu.MenuID : 67;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
  
            return View();
        }

        [HttpGet]
        public IActionResult GetListAllData(DataSourceLoadOptions loadOptions)
        {
            var data = _mesVirtualWarehouseService.GetListAllData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);
        
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult VirtualWareHouseCreatePopup(string VirtualWareHouseId,string Status)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Status = Status;
            var model = new MES_VirtualWarehouse();
            model.Creater = CurrentUser.UserID;
            if (VirtualWareHouseId != null)
            {
                model = _mesVirtualWarehouseService.GetDataDetail(VirtualWareHouseId);
            }
            return PartialView("VirtualWareHouseCreatePopup", model);
        }

        [HttpPost]
        public IActionResult DeleteVirtualWareHouses(string[] VirtualWareHouseId)
        {
            var result = _mesVirtualWarehouseService.DeleteVirtualWarehouse(VirtualWareHouseId);
            return Json(result);
        }

        public IActionResult ShowVirtualWareHousePopupFileUpload(string pagID, string fileID, string pagName)
        {
            FileInforbyID temp = new FileInforbyID();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            temp.Site_ID = CurrentUser.SiteID;
            temp.ID = "FileID" + ViewBag.Thread;
            temp.UrlPath = "";
            temp.Pag_ID = pagID;
            temp.FileMasterID = fileID;         
            temp.Pag_Name = pagName;
            if (fileID == null || fileID == "")
            {
                temp.FileMasterID = Guid.NewGuid().ToString();
                temp.FileID = temp.FileMasterID;
            }
            temp.Form_Name = "Virtual Warehouse";
            temp.Sp_Name = "UPLOAD_FILE_VIRTUALWAREHOUSE";
            temp.extensions = new[] { ".jpg", ".jpeg", ".gif", ".png" };
            temp.Code = "Virtual Warehouse ID";
            temp.Name = "User Name";
            temp.Action_ReloadListFile = "GetVirtualWareHousesDetailByID";
            temp.Controller_ReloadListFile = "MESVirtualWarehouse";
            return PartialView("~/Views/Shared/_PopupFileUpload.cshtml", temp);
        }

        [HttpGet]
        public IActionResult GetVirtualWareHousesDetailByID(string ID)
        {
            var result = _mesVirtualWarehouseService.GetVirtualWareHousesDetailByID(ID);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetItemClassCodeByCategory(DataSourceLoadOptions loadOptions)
        {
            var data = _itemClassService.GetItemClassCodeByCategory();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpPost]
        public IActionResult SaveVirtualWarehouse(string data)
        {
            MES_VirtualWarehouse a = JsonConvert.DeserializeObject<MES_VirtualWarehouse>(data);
            string type = "Update";
            if(a.VirtualWareHouseId == 0)
            {
                //a.VirtualWareHouseId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                type = "Insert";
            }    
            
            var result = _mesVirtualWarehouseService.SaveVirtualWarehouse(a, CurrentUser.UserID, type);
            return Json(result);
        }


        #endregion
        #region SearchVirtualWarehouse
        [HttpGet]
        public object SearchVirtualWarehouse(DataSourceLoadOptions loadOptions, MES_VirtualWarehouse model)
        {
            var result = _mesVirtualWarehouseService.SearchVirtualWarehouse(model);
            //return DataSourceLoader.Load(result, loadOptions);
            return Json(result);
        }
        #endregion
    }
}
