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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Modules.Kiosk.Management.Controllers
{
    public class AdMgtController:BaseController
    {
        static HttpClient client = new HttpClient();
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOVoiceFileRepository _voiceFileService;
        private readonly IUserService _userService;
        private readonly IKIOAdMgtRepository _adMgtRepository;
        public AdMgtController(IMapper mapper, IKIOVoiceFileRepository voiceFileService, IKIOAdMgtRepository adMgtRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._voiceFileService = voiceFileService;
            this._userService = userService;
            this._adMgtRepository = adMgtRepository;
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

        public IActionResult ShowDetailAdMgt(string adNo, string viewbagIndex, int menuParent)
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

            var adMgt = new KIO_AdMgt();
            adMgt.attachFilePath = "test/test";
            if (adNo != null)
            {
                var searchMgt = new KIO_AdMgt();
                searchMgt.adNo = Convert.ToInt32(adNo);
                adMgt = _adMgtRepository.GetAdMgt(searchMgt).FirstOrDefault();
            }
            return PartialView("ShowDetailAdMgt", adMgt);

        }
        #endregion

        #region Get Data
        public ActionResult<List<KIO_AdMgt>> GetAdMgt(KIO_AdMgt adMgt)
        {
            var listAd = _adMgtRepository.GetAdMgt(adMgt);
            return Json(listAd);
        }
        public ActionResult<List<KIO_StoreMaster>> GetAdMgtStore(string adNo)
        {
            var listAdStore = _adMgtRepository.GetAdMgtStore(adNo);
            return Json(listAdStore);
        }
        [HttpGet]
        public IActionResult GetAdImage(string adNo)
        {
            try
            {
                var adSearch = new KIO_AdMgt();
                adSearch.adNo = Convert.ToInt16(adNo);
                var ad = _adMgtRepository.GetAdMgt(adSearch).SingleOrDefault();
                if(ad == null)
                {
                    return BadRequest();
                }
                return base.File(System.IO.File.ReadAllBytes(ad.attachFilePath), "image/jpeg");
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public ActionResult<object> LoadImageAd(string adNo)
        {
            try
            {
                if (adNo == null)
                {
                    return Json(new { success = false, data = "", message = "" });
                }
                var temp = new KIO_AdMgt();
                temp.adNo = Convert.ToInt16(adNo);

                var ads = _adMgtRepository.GetAdMgt(temp).SingleOrDefault();
                if(ads == null)
                {
                    return Json(new { success = false, data = "", message = "" });
                }

                string base64img = Convert.ToBase64String(System.IO.File.ReadAllBytes(ads.attachFilePath.Trim()));
                return Json(new { success = true, data = "data:image/png;base64," + base64img, message="" });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, data = "", message = ex.Message });
            }
        }
        #endregion

        #region Create - Update - Delete
        public ActionResult<Result> SaveAdMgt(KIO_AdMgt adMgt, string listStoreNo)
        {
            try
            {
                if (adMgt.attachFilePath.Contains("temp"))
                {
                    string fileName = adMgt.attachFilePath.Split("/")[adMgt.attachFilePath.Split("/").Length - 1];
                    string pathImg = Path.Combine(Directory.GetCurrentDirectory(), "uploads/advertistment/" + fileName);

                    if (System.IO.File.Exists(adMgt.attachFilePath))
                    {
                        System.IO.File.Copy(adMgt.attachFilePath, pathImg);
                        System.IO.File.Delete(adMgt.attachFilePath);
                        adMgt.attachFilePath = pathImg;
                    }
                }
                return Json(_adMgtRepository.SaveAdMgt(adMgt, listStoreNo));
            }
            catch(Exception ex)
            {
                return new Result { Success = false, Message =  ex.Message};
            }

        }
        [HttpGet]
        public async Task<Result> SendAsynchronous(int adNo, string adType, string adName, int adTemp)
        {

            try
            {
                HttpResponseMessage httpResponse;

                var adDto = new KIO_AdMgt() { adNo = adNo, adType = adType };
                var ad = _adMgtRepository.GetAdMgt(adDto).SingleOrDefault();
                var adMapping = new tblAdMgtInfo()
                {
                    AdNo = ad.adNo,
                    AdType = ad.adType,
                    AdName = ad.adName,
                    PeriodStartDate = Convert.ToDateTime(ad.periodStartDate),
                    PeriodEndDate = Convert.ToDateTime(ad.periodEndDate),
                    DayStartTime = Convert.ToDateTime(ad.dayStartTime),
                    DayEndTime = Convert.ToDateTime(ad.dayEndTime),
                    AdStatus = Convert.ToBoolean(ad.adStatus),
                    RegistDate = Convert.ToDateTime(ad.registDate),
                    ResitUser = ad.resitUser,
                    AdLocation = Convert.ToBoolean(ad.adLocation),
                    AttachFilePath = ad.attachFilePath,
                    Version = ad.version,
                    LocalPath = "",
                    Check = false

                };
                RequestInfo req = new RequestInfo();
                req.Data = adMapping;

                DataRequest adData = new DataRequest()
                {
                    Signature = 101,
                    FrameID = 0,
                    FunctionCode = Convert.ToUInt16(adTemp == adNo ? 8200 : 8198),
                    DataLength = 10000,
                    Data = req
                };
                try
                {
                    httpResponse = await client.PostAsJsonAsync("http://localhost:5001/Kiosk/KioskService/GetData", adData);
                    var data = JsonConvert.SerializeObject(adData);
                    //response.EnsureSuccessStatusCode();
                    return new Result { Data = "", Message = MessageCode.MD0008, Success = true };
                }
                catch
                {
                    return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
                }
            }
            catch
            {
                return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
            }
        }
        public ActionResult<Result> DeleteAdMgt(string adNo, bool checkDeleteAd)
        {
            return Json(_adMgtRepository.DeleteAdMgt(adNo, checkDeleteAd));
        }
        [HttpPost]
        public ActionResult<object> UploadImageAd()
        {
            try
            {
                var myFile = Request.Form.Files["image_ad"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/advertistment/temp/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string imagePath = DateTime.Now.ToString("yyyyMMddhhmmss") + myFile.FileName;
                using (var fileStream = System.IO.File.Create(Path.Combine(path, imagePath)))
                {
                    myFile.CopyTo(fileStream);
                }
                string base64img = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(path, imagePath)));
                return Json(new { data = "data:image/png;base64," + base64img, path = Path.Combine(path, imagePath).Trim()});
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
