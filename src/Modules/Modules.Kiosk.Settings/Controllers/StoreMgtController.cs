using AutoMapper;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Kiosk.Management.Repositories.IRepositories;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Modules.Kiosk.Management.Controllers
{
    public class StoreMgtController : BaseController
    {
        static HttpClient client = new HttpClient();
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOVoiceFileRepository _voiceFileService;
        private readonly IUserService _userService;
        private readonly IKIOStoreRepository _storeRepository;
        private readonly IKIOCommonCodeRepository _commonCodeRepository;

        public object HttpMessageResponse { get; private set; }

        public StoreMgtController(IMapper mapper, IKIOVoiceFileRepository voiceFileService, IKIOStoreRepository storeRepository,
            IKIOCommonCodeRepository commonCodeRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._voiceFileService = voiceFileService;
            this._userService = userService;
            this._storeRepository = storeRepository;
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

        public IActionResult ShowUserStoreMgtDetail(string storeNo, string userId, string viewbagIndex, int menuParent)
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
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserId = CurrentUser.UserID;

            var storeUserMgt = new KIO_UserStore();
            try
            {
                var code = _commonCodeRepository.GetCommonCode(null, "DPSS01", true).SingleOrDefault();
                storeUserMgt.password = code.commonSubName1;
            }
            catch
            {
                storeUserMgt.password = "123456";
            }
            if (storeNo != null)
            {
                storeUserMgt = _storeRepository.GetUserStoreMgt(storeNo, userId, "USTP03").SingleOrDefault();
            }
            return PartialView("ShowUserStoreMgtDetail", storeUserMgt);

        }
        #endregion

        #region Get Data
        [HttpGet]
        public ActionResult<List<KIO_StoreMaster>> GetStoreMastersByUser(string siteType)
        {
            var stores = new List<KIO_StoreMaster>();
            if (CurrentUser.UserType == "G000C001" || CurrentUser.UserType == "G000C002")
            {
                stores = _storeRepository.GetStoreMasters(null, null, null, siteType);
            }
            else
            {
                stores = _storeRepository.GetStoreMastersByUser(CurrentUser.UserCode);
            }
            return Json(stores);
        }
        public ActionResult<List<KIO_StoreMaster>> GetHistoryManager(string storeNo, string userId)
        { 
            return Json(_storeRepository.GetHistoryManager(storeNo, userId));
        }
        public ActionResult<List<KIO_StoreMaster>> GetStoreMasters(string storeNo, string location, string storeName, string siteType)
        {
            var storeMas = _storeRepository.GetStoreMasters(storeNo, location, storeName, siteType);
            return Json(storeMas);
        }
        public ActionResult<List<KIO_StoreMaster>> GetStoreMaster(string storeNo, string location, string storeName, string siteType)
        {
            var storeMas = _storeRepository.GetStoreMasters(storeNo, location, storeName, siteType);
            storeMas.ForEach(x => x.no = 0);
            return Json(storeMas);
        }

        public ActionResult<List<KIO_StoreDevice>> GetStoreDevices(string storeNo, string deviceName, string type, string key, string ip, string storeDeviceNo)
        {
            var storeMas = _storeRepository.GetStoreDevices(storeNo, deviceName, type, key, ip, storeDeviceNo);
            return Json(storeMas);
        }
        [HttpGet]
        public ActionResult<List<KIO_UserStore>> GetUserStoreMgt(string storeNo, string userId)
        {
            return Json(_storeRepository.GetUserStoreMgt(storeNo, userId, "USTP03"));
        }

        public ActionResult<KIO_UserStore> GetUserById(string userId)
        {
            return Json(_storeRepository.GetUserById(userId));
        }
        #endregion

        #region Create - Update - Delete
        public async Task<Result> RequestBlockKiosk(int storeNo, int storeDeviceNo, bool status)
        {
            try
            {
                HttpResponseMessage httpResponse;
                var storeInfo = new StoreDeviceDto() { storeNo = storeNo, storeDeviceNo = storeDeviceNo, deviceStatus = status };
                var dataObj = new Datas()
                {
                    Data = storeInfo
                };
                DataRequest storeData = new DataRequest()
                {
                    Signature = 106,
                    FrameID = 0,
                    FunctionCode = 8241,
                    DataLength = 10000,
                    Data = dataObj
                };
                try
                {
                    httpResponse = await client.PostAsJsonAsync("http://api.owlgardien.com:81/Kiosk/KioskService/GetData", storeData);

                    httpResponse.EnsureSuccessStatusCode();
                    return new Result { Data = "", Message = "", Success = true };
                }
                catch
                {
                    return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
                }
            }
            catch
            {
                return new Result { Data = "", Message = "Error in system, may be lose connection to server kiosk! Please try again", Success = false };
            }
        }
        public ActionResult<Result> SaveStoreMgt(KIO_UserStore userStore)
        {
            int siteId = 0;
            if (CurrentUser.UserType != "G000C001")
            {
                siteId = CurrentUser.SiteID;
            }
            var code = _commonCodeRepository.GetCommonCode(null, "DPSS01", true).SingleOrDefault();
            string pass = code == null ? "123456" : code.commonSubName1;
            string uid = Guid.NewGuid().ToString();
            userStore.userType = "USTP03";
            var result = _storeRepository.SaveStoreMgt(userStore, siteId, uid, pass);
            return Json(result);
        }
        public async Task<Result> RequestStoreMaster(string storeMaster, int storeNo)
        {
            try
            {
                HttpResponseMessage httpResponse;
                RequestInfo request = new RequestInfo();
                KIO_StoreMaster store = JsonConvert.DeserializeObject<KIO_StoreMaster>(storeMaster);
                tblStoreMasterInfo storeInfo = new tblStoreMasterInfo()
                {
                    StoreNo = store.storeNo,
                    Location = store.location,
                    StoreName = store.storeName,
                    BizNumber = store.bizNumber,
                    ZipCode = store.zipCode,
                    Address1 = store.address1,
                    Address2 = store.address2,
                    BizPhoneNumber = store.bizPhoneNumber,
                    Memo = store.memo,
                    RegistDate = Convert.ToDateTime(store.registDate),
                    MonitoringStartime = Convert.ToDateTime(store.monitoringStartime),
                    MonitoringEndtime = Convert.ToDateTime(store.monitoringEndtime),
                };
                request.Data = storeInfo;
                DataRequest storeData = new DataRequest
                {
                    Signature = 117,
                    FrameID = 0,
                    FunctionCode = Convert.ToUInt16(store.storeNo == storeNo ? 8200 : 8198),
                    DataLength = 10000,
                    Data = request
                };
                try
                {
                    httpResponse = await client.PostAsJsonAsync("http://api.owlgardien.com:81/Kiosk/KioskService/GetData", storeData);
                    httpResponse.EnsureSuccessStatusCode();
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
        public ActionResult<Result> SaveStoreMaster(KIO_StoreMaster storeMaster)
        {
            var result = _storeRepository.SaveStoreMaster(storeMaster);
            return Json(result);
        }
        public ActionResult<Result> ChangesPassword(string password, string userId, string flag)
        {
            try
            {
                var code = _commonCodeRepository.GetCommonCode(null, "DPSS01", true).SingleOrDefault();
                if(code == null)
                {
                    return new Result { Success = false, Message = MessageCode.MEPASS01 };

                }
                return flag == "1" ? _storeRepository.ChangesPassword(password, userId) : _storeRepository.ChangesPassword(code.commonSubName1, userId);

            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.ME0003 };
            }
        }
        [HttpGet]
        public ActionResult<KIO_CommonCode> GetDefaultPassword()
        {
            var code = _commonCodeRepository.GetCommonCode(null, "DPSS01", true).SingleOrDefault();
            return Json(code.commonSubName1);
        }

        #endregion
    }
}
