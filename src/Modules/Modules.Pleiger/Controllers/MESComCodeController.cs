using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
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

namespace Modules.Pleiger.Controllers
{
    public class MESComCodeController : BaseController
    {
        #region Prop
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public MESComCodeController(IMESComCodeService mesComCodeService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._mesComCodeService = mesComCodeService;
            this._contextAccessor = contextAccessor;
        }
        #endregion


        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }
        #region "Get Data"

        #region CommonCode MST
        
        [HttpGet]
        public IActionResult GetListComCodeMST(DataSourceLoadOptions loadOptions)
        {
            var data = _mesComCodeService.GetListComCodeMST();
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        #endregion

        #region CommonCode DTL
        [HttpGet]
        public IActionResult GetListComCodeDTL(DataSourceLoadOptions loadOptions, string groupCD)
        {
            var data = _mesComCodeService.GetListComCodeDTL(groupCD);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        public IActionResult GetListComCodeDTLAll(DataSourceLoadOptions loadOptions, string groupCD)
        {
            var data = _mesComCodeService.GetListComCodeDTLAll(groupCD);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        public IActionResult GetListComCodeDTLProduct(DataSourceLoadOptions loadOptions)
        {
            var data = _mesComCodeService.GetListComCodeDTLProduct();
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }


        #endregion
        #endregion


        #region "CRUD Data"

        #region CommonCode MST
        // Save SYUser Group
        [HttpPost]
        public IActionResult SaveDataListComCodeMST(List<MES_ComCodeMst> dataMST, List<MES_ComCodeDtls>  dataDTL,
                                List<MES_ComCodeDtls> dataDTLDelete, string groupCdSelected)
        {
            Result result = _mesComCodeService.SaveDataComCodeMST(dataMST, dataDTL, dataDTLDelete, groupCdSelected, CurrentUser);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeleteDataComCodeMST(MES_ComCodeMst dataMST)
        {
            Result result = _mesComCodeService.DeleteGroupComCodeMST(dataMST, CurrentUser);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region CommonCode DTL
        #endregion

        #endregion

    }

}
