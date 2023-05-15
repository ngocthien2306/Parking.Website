using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace Modules.Kiosk.Management.Controllers
{
    public class PreferencesController : BaseController
    {
        static HttpClient client = new HttpClient();
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOVoiceFileRepository _voiceFileService;
        private readonly IKIOPreferenceRepository _preferenceRepository;
        private readonly IUserService _userService;
        public PreferencesController(IMapper mapper, IKIOVoiceFileRepository voiceFileService, IKIOPreferenceRepository preferenceRepository,
        IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._voiceFileService = voiceFileService;
            this._userService = userService;
            this._preferenceRepository = preferenceRepository;
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
        public ActionResult<List<KIO_StoreEnvSettings>> GetStoreEnvSettings(string storeNo, string envNo)
        {
            return Json(_preferenceRepository.GetStoreEnvSettings(storeNo, envNo));
        }

        [HttpGet]
        public ActionResult<List<KIO_AlarmToStoreMessage>> GetAlarmToStoreMessage(string storeNo, string alarmNo)
        {
            return Json(_preferenceRepository.GetAlarmToStoreMessage(storeNo, alarmNo));
        }
        #endregion

        #region Create - Update - Delete
        [HttpPost]
        public ActionResult<Result> SaveEnvSettings(KIO_StoreEnvSettings envSettings)
        {
            return Json(_preferenceRepository.SaveEnvSettings(envSettings));
        }
        public ActionResult<Result> SaveAlarmStoreMessage(KIO_AlarmToStoreMessage alarmToStoreMessage)
        {
            return Json(_preferenceRepository.SaveAlarmStoreMessage(alarmToStoreMessage));
        }
        [HttpGet]
        public async Task<Result> RequestSettings(string storeNo)
        {
            try
            {
                HttpResponseMessage responseMessage;
                var env = _preferenceRepository.GetStoreEnvSettings(storeNo, null).SingleOrDefault();

                var envMapping = new tblStoreEnvironmentSettingInfo()
                {
                    EnvironmentSettingNo = env.environmentSettingNo,
                    StoreNo = env.storeNo,
                    CertifCriteria = env.certifCriteria,
                    PhoneInput = env.phoneInput,
                    SimilarityRateApproval = env.similarityRateApproval,
                    AuthAfterCompleted = env.authAfterCompleted,
                    AuthAfterCardId = env.authAfterCardId,
                    EId = env.eId,
                    UseCamera = env.useCamera,
                    UseScanner = env.useScanner
                };
                RequestInfo req = new RequestInfo();
                req.Data = envMapping;

                DataRequest envData = new DataRequest()
                {
                    Signature = 111,
                    FrameID = 0,
                    FunctionCode = 8200,
                    DataLength = 10000,
                    Data = req
                };

                try
                {
                    responseMessage = await client.PostAsJsonAsync("http://api.owlgardien.com:81/Kiosk/KioskService/GetData", envData);
                    responseMessage.EnsureSuccessStatusCode();
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
        public ActionResult<Result> SaveDataPreferences(string env, string alarm)
        {
            KIO_StoreEnvSettings envs = JsonSerializer.Deserialize<KIO_StoreEnvSettings>(env);
            KIO_AlarmToStoreMessage alarms = JsonSerializer.Deserialize<KIO_AlarmToStoreMessage>(alarm);
            var result = _preferenceRepository.SaveEnvSettings(envs);
            if (result.Success == false) return Json(result);
            result = _preferenceRepository.SaveAlarmStoreMessage(alarms);
            return Json(result);
        }
        #endregion
    }
}
