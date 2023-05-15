using AutoMapper;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Kiosk.Management.Controllers
{
    public class EquipmentSettingController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOVoiceFileRepository _voiceFileService;
        private readonly IUserService _userService;
        private readonly IKIOEquipmentRepository _equipmentRepository;
        private readonly IKIOStoreRepository _storeRepository;
        public EquipmentSettingController(IMapper mapper, IKIOVoiceFileRepository voiceFileService, IKIOEquipmentRepository equipmentRepository,
        IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService,
        IKIOStoreRepository storeRepository) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._voiceFileService = voiceFileService;
            this._userService = userService;
            this._storeRepository = storeRepository;
            this._equipmentRepository = equipmentRepository;
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
        public IActionResult ShowStoreDeviceDetail(string storeDeviceNo, string viewbagIndex, int menuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            //var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            //ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserId = CurrentUser.UserID;
            ViewBag.UserType = CurrentUser.SystemUserType;
            var storeDevice = new KIO_StoreDevice();
            if (storeDeviceNo != null)
            {
                storeDevice = _storeRepository.GetStoreDevices(null, null, null, null, null, storeDeviceNo).FirstOrDefault();
            }
            return PartialView("ShowStoreDeviceDetail", storeDevice);

        }
        #endregion

        #region CRUD Store Device
        [HttpPost]
        public ActionResult<Result> UpdateStatusDevice(string storeNo, string storeDeviceNo, bool status) 
        {
            var result = _equipmentRepository.UpdateStatusDevice(storeNo, storeDeviceNo, status);
            return Json(result);
        }
        [HttpPost]
        public ActionResult<Result> SaveDataStoreDevice(KIO_StoreDevice storeDevice)
        {
            var result = _equipmentRepository.SaveDataStoreDevice(storeDevice, CurrentUser.UserID);
            return Json(result);
        }
        [HttpDelete]
        public ActionResult<Result> DeleteStoreDevice(string storeDeviceNo)
        {
            var result = _equipmentRepository.DeleteStoreDevice(storeDeviceNo);
            return Json(result);
        }
        [HttpPost]
        public ActionResult<object> GetPathDownloadRemoteProgram(string rdpPath)
        {
            try
            {
                string filenamebmp = "";
                string path = rdpPath == null ? "remote-window-default.rdp" : rdpPath.Replace(" ", "").Replace(":", "-") + ".rdp";
                filenamebmp = Path.Combine(Directory.GetCurrentDirectory() + "\\resources\\remote\\" + path);

                if (!System.IO.File.Exists(filenamebmp))
                {
                    using (var stream = System.IO.File.Open(filenamebmp, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    {
                        var remote = RemoteProgramData.RemoteData();
                        remote[23] = "full address:s:"+rdpPath;
                        byte[] remoteAsBytes = remote.SelectMany(s => Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                        stream.WriteAsync(remoteAsBytes);
                    }
                  
                    return Json(new { result = true, downloadRemotePath = filenamebmp, fileName = path });
                }

                string[] content = System.IO.File.ReadAllLines(filenamebmp);
                content[23] = "full address:s:"+ rdpPath;
                System.IO.File.WriteAllLines(filenamebmp, content);
                return Json(new { result = true, downloadRemotePath = filenamebmp, fileName = path });
            }
            catch
            {
                return Json(new { result = false, downloadRemotePath = "", fileName = "" });
            }
        }
        [HttpGet]
        public async Task<ActionResult> DownloadRemoteProgram(string fileLink, string fileName)
        {
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
                return new EmptyResult();
            }
        }

        #endregion
    }
}
