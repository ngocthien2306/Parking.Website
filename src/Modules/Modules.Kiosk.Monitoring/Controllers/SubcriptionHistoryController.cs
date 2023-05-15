using AutoMapper;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Monitoring.Controllers
{
    public class SubcriptionHistoryController : BaseController
    {

        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IKIOSubscriptionRepository _kioSubscriptionRepository;

        public SubcriptionHistoryController(IMapper mapper, IKIOSubscriptionRepository kioSubscriptionRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._kioSubscriptionRepository = kioSubscriptionRepository;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
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

        #region Get data
        [HttpGet]
        public ActionResult<List<KIO_SubscriptionHistory>> GetSubscriptionHistory(string storeNo, string startTime, string endTime)
        {
            List<KIO_SubscriptionHistory> subs = new List<KIO_SubscriptionHistory>();
            try
            {
                if (startTime == null || endTime == null) subs = _kioSubscriptionRepository.GetSubscriptionHistory(storeNo, startTime, endTime);
                else subs = _kioSubscriptionRepository.GetSubscriptionHistory(storeNo, startTime, endTime.Split()[0] + " " + DateTime.Now.ToString("HH:mm:ss"));

                return Json(subs);
            }
            catch
            {
                return subs;
            }
        }
        #endregion


        #region Create - Update - Remove

        #endregion
    }
}
