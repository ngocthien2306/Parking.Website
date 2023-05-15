using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Management.Repositories.IRepositories;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Text;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;


namespace Modules.Kiosk.Management.Controllers
{
    public class CommonCodeMgtController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IKIOCommonCodeRepository _commonCodeRepository;
        public CommonCodeMgtController(IMapper mapper, IKIOCommonCodeRepository commonCodeRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
            this._commonCodeRepository = commonCodeRepository;
        }
        #region View

        public ActionResult Index()
        {

                ViewBag.Thread = Guid.NewGuid().ToString("N");
                var curUrlTemp = (Request.Path.Value + Request.QueryString);
                var curUrl = URLRequest.URLSubstring(curUrlTemp);
                var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

                var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
                if (ListButtonPermissionByUser.Count > 0)
                {
                    ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                    ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                    ViewBag.EDIT_YN = ListButtonPermissionByUser[0].EDIT_YN;
                    ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
                    ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                    ViewBag.UPLOAD_FILE_YN = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
                }
                ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
                ViewBag.CurrentUser = CurrentUser;
                return View();
            
        }
        #endregion

        #region Get Data
        [HttpGet]

        public ActionResult<List<KIO_CommonCode>> GetCommonCode(string code, string subCode, bool status)
        {
            var listCode = _commonCodeRepository.GetCommonCode(code, subCode, status);
            return Json(listCode);
        }
        [HttpGet]
        public ActionResult<List<KIO_MasterCode>> GetMasterCode(string code, bool status)
        {
            return Json(_commonCodeRepository.GetMasterCode(code, status));
        }
        #endregion

        #region Create - Update - Delete
        [HttpPost]
        public ActionResult<Result> SaveCommonCode(string values, string code)
        {
            var commonCode = new KIO_CommonCode();
            commonCode.commonCode = code;
            JsonConvert.PopulateObject(values, commonCode);
            return Json(_commonCodeRepository.SaveCommonCode(commonCode));
        }
        [HttpPut]
        public ActionResult<Result> UpdateCommonCode(string key, string values, string code)
        {
            var commonCode = _commonCodeRepository.GetCommonCode(code, key, false).SingleOrDefault();
            JsonConvert.PopulateObject(values, commonCode);
            return Json(_commonCodeRepository.SaveCommonCode(commonCode));
        }
        [HttpPost]
        public ActionResult<Result> SaveMasterCode(string values)
        {
            var masterCode = new KIO_MasterCode();
            JsonConvert.PopulateObject(values, masterCode);         
            return Json(_commonCodeRepository.SaveMasterCode(masterCode));
        }
        [HttpPut]
        public ActionResult<Result> UpdateMasterCode(string key, string values)
        {
            var masterCode = _commonCodeRepository.GetMasterCode(key, false).SingleOrDefault();
            JsonConvert.PopulateObject(values, masterCode);
            var result = _commonCodeRepository.SaveMasterCode(masterCode);
            return Json(result);
        }
        [HttpDelete]
        public ActionResult<Result> DeleteCommonCode(string key)
        {
            return Json(_commonCodeRepository.DeleteCommonCode(key));
        }
        [HttpDelete]
        public ActionResult<Result> DeleteListCommonCode(string[] cmCodes)
        {
            Result result = new Result();
            foreach(string cmCode in cmCodes)
            {
                result = _commonCodeRepository.DeleteCommonCode(cmCode);
                if (!result.Success)
                {
                    return result;
                }
            }
            return Json(result);
        }
        [HttpDelete]
        public ActionResult<Result> DeleteMasterCode(string key)
        {
            var result = _commonCodeRepository.DeleteMasterCode(key);
            return Json(result);
        }
        #endregion
    }
}
