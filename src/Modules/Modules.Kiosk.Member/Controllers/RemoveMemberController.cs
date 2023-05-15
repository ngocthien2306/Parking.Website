using AutoMapper;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Member.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Member.Controllers
{
    public class RemoveMemberController : BaseController
    {


        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IKIOMemberRemoveMgt _memberRemoveMgt;
        public RemoveMemberController(IMapper mapper, IKIOMemberRemoveMgt memberRemoveMgt,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
            this._memberRemoveMgt = memberRemoveMgt;
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
        public IActionResult ShowMemberRemoveDetail(string storeNo, string userId, string ViewbagIndex, int MenuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, MenuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            //var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            //ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = ViewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;

            var member = new KIO_SubscriptionHistory();
            if (storeNo != null)
            {
                member = _memberRemoveMgt.GetMemberRemove(storeNo, userId, 0, 0).FirstOrDefault();

                //member.takenPhoto = null;
                //member.idCardPhoto = null;
            }
            return PartialView("ShowMemberRemoveDetail", member);

        }
        #endregion

        #region Get Data
        public ActionResult<List<KIO_SubscriptionHistory>> GetMemberRemove(string storeNo, string userId, int lessMonth, int onceRecently)
        {
            var listMember = _memberRemoveMgt.GetMemberRemove(storeNo, userId, lessMonth, onceRecently);
            return Json(listMember);
        }
        [HttpGet]
        public ActionResult<object> GetImageMemberRemove(string storeNo, string userId)
        {
            try
            {
                var member = _memberRemoveMgt.GetMemberRemoveDetail(storeNo, userId).FirstOrDefault();
                var takenPhotoBase64 = member.takenPhoto == null ? "data:image/png;base64," : "data:image/png;base64," + Convert.ToBase64String(member.takenPhoto).ToString();
                var idCardPhotoBase64 = member.idCardPhoto == null ? "data:image/png;base64," : "data:image/png;base64," + Convert.ToBase64String(member.idCardPhoto).ToString();
                return Json(new { takenPhotoBase64 = takenPhotoBase64, idCardPhotoBase64 = idCardPhotoBase64 });
            }
            catch
            {
                return Json(new { takenPhotoBase64 = "data:image/png;base64,", idCardPhotoBase64 = "data:image/png;base64," });
            }

        }

        [HttpGet]
        public ActionResult<List<KIO_UserHistory>> GetRemoveHistory(string userId)
        {
            var listUserHistory = _memberRemoveMgt.GetRemoveHistory(userId);
            return Json(listUserHistory);
        }
        #endregion

        #region Create - Update - Delete
        [HttpDelete]
        public ActionResult<Result> DeleteMemberRemove(string userId, string storeNo)
        {
            return Json(_memberRemoveMgt.DeleteMemberRemove(storeNo, userId));
        }
        [HttpDelete]
        public ActionResult<Result> RemoveListMember(string userIds)
        {
            try
            {
                List<KIO_UserIdDto> userDto = JsonConvert.DeserializeObject<List<KIO_UserIdDto>>(userIds);
                List<KIO_UserIdDto> cacheUserId = null;

                userDto.ForEach(u =>
                {
                    var result = _memberRemoveMgt.DeleteMemberRemove(null, u.userId);
                    if (!result.Success)
                    {
                        cacheUserId.Add(new KIO_UserIdDto { userId = u.userId, userName = u.userName });
                    }
                });
                if (cacheUserId != null)
                {
                    string msg = "";
                    cacheUserId.ForEach(u => msg += "[" + u.userName + "]");
                    return new Result { Success = false, Message = "Delele was failed! Can not delete user  " + msg };
                }
                else
                {
                    return new Result { Success = true, Message = "Delete data successfully!" };
                }
            }
            catch
            {
                return new Result { Success = false, Message = "Delele data failly!" };
            }

        }
        [HttpPost]
        public ActionResult<Result> SaveMemberRemove(SaveUserDto saveUserDto)
        {
            return Json(_memberRemoveMgt.SaveMemberRemove(saveUserDto));
        }
        [HttpPost]
        public ActionResult<object> UploadImageTaken(string userId)
        {
            try
            {
                var myFile = Request.Form.Files["imageFileTaken"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/" + userId);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                {
                    myFile.CopyTo(fileStream);
                }
                string base64img = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(path, myFile.FileName)));
                return Json(new { data = "data:image/png;base64," + base64img, path = Path.Combine(path, myFile.FileName.Trim()) });
            }
            catch
            {
                Response.StatusCode = 400;
                return new EmptyResult();
            }
        }
        [HttpPost]
        public ActionResult<object> UploadImageCard(string userId)
        {
            try
            {
                var myFile = Request.Form.Files["imageFileCard"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/" + userId);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                {
                    myFile.CopyTo(fileStream);
                }
                string base64img = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(path, myFile.FileName)));
                return Json(new { data = "data:image/png;base64," + base64img, path = Path.Combine(path, myFile.FileName.Trim()) });
            }
            catch
            {
                Response.StatusCode = 400;
                return new EmptyResult();
            }
        }
        #endregion
    }
}
