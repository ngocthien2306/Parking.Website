using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.FileUpload.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-07-08
/// 거래처관리 (Customer, Partner Company information)
/// Design v0.3 slide 6: show detail by manual 
/// </summary>
namespace Modules.Pleiger.Controllers
{
    public class MESPartnerController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IMESPartnerService _partnerService;
        private readonly IMESComCodeService _mesComCodeService;

        public MESPartnerController(IHttpContextAccessor contextAccessor, IMESPartnerService partnerService, IMESComCodeService mesComCodeService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._partnerService = partnerService;
            this._mesComCodeService = mesComCodeService;
        }
        /// <summary>
        /// Mapping with setting in DB page PARTNER_MGT_DETAIL_POPUP, type = G001C004, view custom name = ~/Views/MESPartner/PartnerDetail.cshtml
        /// </summary>
        /// <returns></returns>
        public IActionResult PartnerDetail()
        {
            return View();
        }

        public IActionResult ShowPartnerPopupFileUpload(string pagID, string fileID, string pagName)
        {
            FileInforbyID temp = new FileInforbyID();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            temp.Site_ID = CurrentUser.SiteID;
            temp.ID = "FileID" + ViewBag.Thread;
            temp.UrlPath = "";
            temp.Pag_ID = pagID;
            temp.FileMasterID = fileID;
            temp.Pag_Name = pagName;
            temp.Form_Name = "Partner Upload File";
            temp.Sp_Name = "UPLOAD_FILE_PARTNER";
            if (fileID == null || fileID == "")
            { 
                temp.FileMasterID = Guid.NewGuid().ToString();
                temp.FileID = temp.FileMasterID;
            }
            temp.extensions = Array.Empty<String>();
            temp.Code = "Partner Code";
            temp.Name = "Partner Name";
            temp.Action_ReloadListFile = "GetPartnerDetailByPartnerCode";
            temp.Controller_ReloadListFile = "MESPartner";
            return PartialView("~/Views/Shared/_PopupFileUpload.cshtml", temp);

        }

        public IActionResult IndexVC()
        {
            return ViewComponent("PriorityList", new { maxPriority = 3, isDone = false });
        }

        #region Get Data
        [HttpGet]
        public IActionResult GetPartnerDetailByPartnerCode(string ID)
        {
            var result = _partnerService.GetPartnerDetailByPartnerCode(ID);
            return Json(result);
        }

        [HttpGet]
        public object GetPartnerByPartnerCode(DataSourceLoadOptions loadOptions, string PartnerCode)
        {

            var lstType = _partnerService.GetPartnerDetailByPartnerType(PartnerCode);
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        //public object GetPartnerByPartnerCodeNew(DataSourceLoadOptions loadOptions, string PartnerCode)
        //{

        //    var lstType = _partnerService.GetPartnerByPartnerCode(PartnerCode);
        //    return DataSourceLoader.Load(lstType, loadOptions);
        //}
        [HttpGet]
        public object GetPartnerDetailByPartnerType(DataSourceLoadOptions loadOptions, string PartnerType)
        {

            List<MES_Partner> lstType = new List<MES_Partner>();
            lstType = _partnerService.GetPartnerDetailByPartnerType(PartnerType);
            var loadResult = DataSourceLoader.Load(lstType, loadOptions);
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        //public object GetPartnerDetailByPartnerType(DataSourceLoadOptions loadOptions, string PartnerType)
        //{

        //    List<MES_Partner> lstType = new List<MES_Partner>();
        //    lstType = _partnerService.GetPartnerDetailByPartnerType(PartnerType);
        //    var loadResult = DataSourceLoader.Load(lstType, loadOptions);
        //    return DataSourceLoader.Load(lstType, loadOptions);
        //}
        public object GetPartnerDetailByTwoPartnerType(DataSourceLoadOptions loadOptions, string PartnerType1, string PartnerType2)
        {

            List<MES_Partner> lstType = new List<MES_Partner>();
            lstType = _partnerService.GetPartnerDetailByTwoPartnerType(PartnerType1, PartnerType2);
            var loadResult = DataSourceLoader.Load(lstType, loadOptions);
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        // Get list PartnerType Common Code
        [HttpGet]
        public IActionResult GetListPartnerType(string groupCode)
        {
            var data = _mesComCodeService.GetListComCodeDTL(groupCode);
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        #endregion
        [HttpGet]
        public object GetAllPartner(DataSourceLoadOptions loadOptions)
        {
            var lstType = _partnerService.GetAllPartner();
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        

    }
}
