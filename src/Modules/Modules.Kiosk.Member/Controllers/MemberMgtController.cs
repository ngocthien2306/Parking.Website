using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Protobuf.Collections;
using InfrastructureCore;
using InfrastructureCore.Extensions;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Kiosk.Member.Repositories.IRepository;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Parking.Helper;
using Modules.Pleiger.CommonModels.Models;
using Modules.Pleiger.CommonModels.Parking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace Modules.Kiosk.Member.Controllers
{
    public class MemberMgtController : BaseController
    {
        static HttpClient client = new HttpClient();
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService _userService;
        private readonly IKIOMemberManagement _memberManagement;
        private readonly IKIOCheckInRepository _checkins;
        private readonly IKIOStoreRepository _storeRepository;
        public MemberMgtController(IMapper mapper, IKIOMemberManagement memberManagement, IKIOCheckInRepository checkins, IKIOStoreRepository storeRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this._checkins = checkins;
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._userService = userService;
            this._memberManagement = memberManagement;
            this._storeRepository = storeRepository;
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
        public IActionResult ShowMemberDetailMgt(string storeNo, string userId, string ViewbagIndex, int MenuParent)
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
                member = _memberManagement.GetMemberManagement(storeNo, userId, 0, 0).FirstOrDefault();
                
                //member.takenPhoto = null;
                //member.idCardPhoto = null;
            }
            return PartialView("ShowMemberDetailMgt", member);

        }
        #endregion

        #region Get Data
        public ActionResult<List<KIO_SubscriptionHistory>> GetMemberManagement(string storeNo, string userId, int lessMonth, int onceRecently, string phoneNumber, string username)
        {
            var listMember = _memberManagement.GetMemberManagement(storeNo, userId, lessMonth, onceRecently, phoneNumber, username);
            return Json(listMember);
        }
        [HttpGet]
        public ActionResult<KIO_SubscriptionHistory> GetProfileUser(string userId)
        {
            var member = _memberManagement.GetMemberManagementDetail(null, userId, null).FirstOrDefault();
            if(member == null)
            {
                member = new KIO_SubscriptionHistory();
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/not-found-image.png");
            var noAvailableImage = System.IO.File.ReadAllBytes(path);
            member.takenPhotoBase64 = member.takenPhoto == null ? "data:image/png;base64," +  Convert.ToBase64String(noAvailableImage) : "data:image/png;base64," + Convert.ToBase64String(member.takenPhoto);
            member.idCardFrontPhotoBase64 = member.idCardFrontPhoto == null ? "data:image/png;base64," + Convert.ToBase64String(noAvailableImage) : "data:image/png;base64," + Convert.ToBase64String(member.idCardFrontPhoto);
            member.idCardBackPhotoBase64 = member.idCardBackPhoto == null ? "data:image/png;base64," + Convert.ToBase64String(noAvailableImage) : "data:image/png;base64," + Convert.ToBase64String(member.idCardBackPhoto);

            return Json(member);
        }
        [HttpGet]
        public ActionResult<object> GetImageMember(string storeNo, string userId, string hisNo)
        {
            try
            {
                var member = _memberManagement.GetMemberManagementDetail(storeNo, userId, hisNo).FirstOrDefault();
                var takenPhotoBase64 = member.takenPhoto == null ? "data:image/png;base64," : "data:image/png;base64," + Convert.ToBase64String(member.takenPhoto);
                var idCardPhotoBase64 = member.idCardPhoto == null ? "data:image/png;base64," : "data:image/png;base64," + Convert.ToBase64String(member.idCardPhoto);
                var faceCheckinPhotoBase64 = member.faceCheckIn == null ? "data:image/png;base64," : "data:image/png;base64," + Convert.ToBase64String(member.faceCheckIn);
                return Json(new { takenPhotoBase64 = takenPhotoBase64, idCardPhotoBase64 = idCardPhotoBase64, faceCheckinPhotoBase64 = faceCheckinPhotoBase64 });
            }
            catch
            {
                return Json(new { takenPhotoBase64 = "data:image/png;base64,", idCardPhotoBase64 = "data:image/png;base64," });
            }

        }
        [HttpGet]
        public ActionResult<List<KIO_UserHistory>> GetUserHistory(string userId)
        {
            var listUserHistory = _memberManagement.GetUserHistory(userId);
            return Json(listUserHistory);
        }
        #endregion
        #region Create - Update - Delete
        [HttpGet]
        public async Task<Result> RequestAddOnFaceOk(string userIds)
        {
            try
            {
                HttpResponseMessage httpResponse;
                var userPhoto = new tblUserPhotoInfo();

                List<KIO_UserIdDto> userDto = JsonConvert.DeserializeObject<List<KIO_UserIdDto>>(userIds);
                var userStoreMgt = _storeRepository.GetUserStoreMgt(null, userDto[0].userId, null);
                if(userStoreMgt == null)
                {
                    return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
                }

                var userStore = new tblUserMgtStoreInfo() { UserID = userDto[0].userId, StoreNo = userStoreMgt[0].storeNo };
                try
                {
                    userPhoto.TakenPhoto = _checkins.GetPhotoById(userDto[0].userId).takenPhoto;
                }
                catch
                {
                    return new Result { Data = "", Message = MessageCode.MD0015, Success = false };
                }



                tblUserInfo user = new tblUserInfo()
                {
                    UserID = userDto[0].userId,
                    UserType = "USTP01",
                    Password = "",
                    UserName = userDto[0].userName,
                    PhoneNumber = "",
                    Birthday = DateTime.Now,
                    Email = string.Empty,
                    Gender = true,
                    ApproveReject = true,
                    UserStatus = "USST01",
                    RegistDate = DateTime.Now,
                    isRemoveTempUser = false,
                    Desc = string.Empty,
                    ListUserId = null,
                    TblUserPhotoInfo = userPhoto,
                    TblUserMgtStoreInfo = userStore
                };

                var dataObject = new Datas()
                {
                    Data = user
                };

                DataRequest userMgtData = new DataRequest()
                {
                    Signature = 104,
                    FrameID = 0,
                    FunctionCode = 4104,
                    DataLength = 10000,
                    Data = dataObject
                };
                try
                {
                    httpResponse = await client.PostJsonAsync("http://26.115.12.45:81/Kiosk/KioskService/GetData", userMgtData);
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
                return new Result { Data = "", Message = MessageCode.MD0015, Success = false };

            }
        }
        [HttpGet]
        public async Task<Result> RequestDeleteOnFaceOk(string userIds, bool flag)
        {
            try
            {
                HttpResponseMessage response;
                List<KIO_UserIdDto> userDto = JsonConvert.DeserializeObject<List<KIO_UserIdDto>>(userIds);
                List<KIO_UserIdDto> cacheUserId = null;
                List<string> listIds = new List<string>();
                //List<tblUserInfo> users = new List<tblUserInfo>();
                userDto.ForEach(u =>
                {
                    listIds.Add(u.userId);
                });

                tblUserInfo user = new tblUserInfo()
                {
                    UserID = "",
                    UserType = "USTP01",
                    Password = "",
                    UserName = "",
                    PhoneNumber = "",
                    Birthday = DateTime.Now,
                    Email = string.Empty,
                    Gender = true,
                    ApproveReject = true,
                    UserStatus = "USST01",
                    RegistDate = DateTime.Now,
                    isRemoveTempUser = false,
                    Desc = string.Empty,
                    UseYN = flag,
                    ListUserId = listIds
                };

                var dataObject = new Datas()
                {
                    Data = user
                };

                DataRequest userMgtData = new DataRequest()
                {
                    Signature = 104,
                    FrameID = 0,
                    FunctionCode = 4099,
                    DataLength = 10000,
                    Data = dataObject
                };
                try
                {
                    response = await client.PostJsonAsync("http://26.115.12.45:81/Kiosk/KioskService/GetData", userMgtData);
                    response.EnsureSuccessStatusCode();
                    return new Result { Data = "", Message = MessageCode.MD0008, Success = true };
                }
                catch
                {
                    return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
                }
            }
            catch
            {
                return new Result { Data = "", Message = MessageCode.MD0015, Success = false };
            }
        }
        [HttpDelete]
        public ActionResult<Result> DeleteListMember(string userIds)
        {
            try
            {
                List<KIO_UserIdDto> userDto = JsonConvert.DeserializeObject<List<KIO_UserIdDto>>(userIds);
                List<KIO_UserIdDto> cacheUserId = new List<KIO_UserIdDto>();
                List<KIO_UserIdDto> users = new List<KIO_UserIdDto>();

                userDto.ForEach(u =>
                {
                    var result = _memberManagement.DeleteDataMember(null, u.userId);
                    if(!result.Success)
                    {
                        cacheUserId.Add(new KIO_UserIdDto() { userId = u.userId, userName = u.userName });
                    }
                    else
                    {
                        users.Add(new KIO_UserIdDto() { userId = u.userId, userName = u.userName });
                    }
                });
                if(cacheUserId.Count > 0)
                {
                    return new Result { Data = cacheUserId, Message = "", Success = false };
                }

                return new Result { Data = users, Message = "", Success = true };
            }
            catch
            {
                return new Result { Data = "", Message = "", Success = false };
            }
        }
        [HttpDelete]
        public ActionResult<Result> DeleteDataMember(string userId, string storeNo)
        {
            try
            {
                var result = _memberManagement.DeleteDataMember(null, userId);
                return new Result { Data = userId, Message = MessageCode.MD0008, Success = true };
            }
            catch
            {
                return new Result { Data = userId, Message = MessageCode.MD0015, Success = false };
            }


            //return Json(_memberManagement.DeleteDataMember(storeNo, userId));
        }
        [HttpPost]
        public ActionResult<Result> SaveDataMember(SaveUserDto saveUserDto)
        {
            return Json(_memberManagement.SaveDataMember(saveUserDto));
        }
        public ActionResult<Result> SaveUserProfile(SaveUserProfile userProfile)
        {
            userProfile.userId = CurrentUser.UserCode;
            var res = _memberManagement.SaveUserProfile(userProfile);
            return Json(res);
        }
        [HttpPost]
        public async Task<ActionResult<object>> UploadVehicle(string userId)
        {
            try
            {
                var res = new VehicleResponse();
                var myFile = Request.Form.Files["imageFileVehicle"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/" + userId + "/vehicle");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                {
                    myFile.CopyTo(fileStream);
                }
                string base64img = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(path, myFile.FileName)));


                string baseUrl = "http://26.115.12.45:8005/vehicle";
                ApiCaller.SetBaseUrl(baseUrl);
                Result apiResponse = await ApiCaller.VerifyLicensePlateAsync(base64img);
                // Result apiResponse = new Result { Success = true };
                if (apiResponse.Success)
                {
                    UploadVehicleResponse response = JsonConvert.DeserializeObject<UploadVehicleResponse>(apiResponse.Data.ToString());

                    // Fake data
                    // UploadVehicleResponse response = new UploadVehicleResponse() { license="86B343046", plateType = "2 line", transportType = "Oto" };

                    res.data = "data:image/png;base64," + base64img;
                    res.path = Path.Combine(path, myFile.FileName.Trim());
                    res.success = apiResponse.Success;
                    res.plateNum = response.license;
                    res.transportType = response.transportType;
                    res.plateType = response.plateType;
                    res.message = response.message;
                    return Json(res);
                }
                else
                {
                    res.data = "data:image/png;base64," + base64img;
                    res.path = Path.Combine(path, myFile.FileName.Trim());
                    res.success = apiResponse.Success;
                    return Json(res);
                }
                
            }
            catch
            {
                Response.StatusCode = 400;
                return new EmptyResult();
            }
        }
        [HttpPost]
        public ActionResult<object> UploadImageLicense(string userId)
        {
            try
            {
                var myFile = Request.Form.Files["imageFileLicense"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/" + userId + "/license");
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
        public ActionResult<object> UploadImageTaken(string userId)
        {
            try
            {
                var myFile = Request.Form.Files["imageFaceTaken"];
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
        [HttpPost]
        public ActionResult<object> UploadImageCardId(string userId, int type)
        {
            try
            {
                var myFile = Request.Form.Files["imageFileCardId"];
                string subPath = type == 1 ? "back/" : "front/";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/images/card/" + subPath + userId);
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
