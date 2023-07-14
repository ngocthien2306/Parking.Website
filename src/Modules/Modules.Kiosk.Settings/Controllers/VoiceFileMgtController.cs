using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using InfrastructureCore;
using InfrastructureCore.Extensions;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Admin.Services.ServiceImp;
using Modules.Common.Models;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Ubiety.Dns.Core;
using static System.Net.WebRequestMethods;
using static UnityEngine.iPhone;

//using Microsoft.AspNetCore.Components.Forms;

namespace Modules.Kiosk.Management.Controllers
{
    public class VoiceFileMgtController : BaseController
    {
        static HttpClient client = new HttpClient();
        private readonly IMapper mapper;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IKIOVoiceFileRepository _voiceFileService;
        private readonly IKIOStoreRepository _storeRepository;
        private readonly IUserService _userService;
        public VoiceFileMgtController(IMapper mapper, IKIOVoiceFileRepository voiceFileService, IKIOStoreRepository storeRepository,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor, IUserService userService) : base(contextAccessor)
        {
            this.mapper = mapper;
            this._accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this._voiceFileService = voiceFileService;
            this._userService = userService;
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

        public IActionResult ShowAudioFileDetail(string soundId, string viewbagIndex, int menuParent)
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
            ViewBag.SoundCheck = 0;
            var Audio = new KIO_ClientSoundMgt();
            if(soundId != null)
            {
                ViewBag.SoundCheck = 1;
                Audio = _voiceFileService.GetListAudioFile(soundId, null).FirstOrDefault();
                ViewBag.Path = Audio.localFileLocation;
                try
                {
                    string base64audio = Convert.ToBase64String(System.IO.File.ReadAllBytes(Audio.localFileLocation));
                    ViewBag.DataSound = "data:audio/wav;base64," + base64audio;
                }
                catch
                {
                    ViewBag.DataSound = "";
                }
            }
            return PartialView("ShowAudioFileDetail", Audio);

        }
        public IActionResult ShowHistoryDeployAudio(string soundId, string viewbagIndex, int menuParent)
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
            ViewBag.SoundNo = soundId;

            return PartialView("ShowHistoryDeploy");
        }
        #endregion

        #region Get data
        [HttpGet]
        public ActionResult<object> GetSingleAudioFile(string audioId)
        {
            var audio = _voiceFileService.GetListAudioFile(audioId, null);
            string audioSource = "";
            try
            {
                string base64audio = Convert.ToBase64String(System.IO.File.ReadAllBytes(audio[0].localFileLocation));
                audioSource = "data:audio/wav;base64," + base64audio;
                return Json(new { data = audio, source = audioSource });
            }
            catch
            {
                return Json(new { data = audio, source = audioSource });
            }
        }

        [HttpGet]
        public IActionResult GetSourceFileAudio(string soundNo)
        {
            var audio = _voiceFileService.GetListAudioFile(soundNo, null);
            try
            {
                return Json(new { data = Path.GetExtension(audio[0].localFileLocation),
                    source = System.IO.File.ReadAllBytes(audio[0].localFileLocation), 
                    soundType = audio[0].soundType, soundNo = audio[0].soundNo });
            }
            catch
            {
                Response.StatusCode = 400;
                return new EmptyResult();
            }
        }

        [HttpGet]
        public ActionResult<List<KIO_ClientSoundMgt>> GetListAudioFile(string audioId, string audioName)
        {
            return Json(_voiceFileService.GetListAudioFile(audioId, audioName));
        }
        [HttpGet]
        public ActionResult<List<KIO_StoreDeployHistory>> GetSoundDeployHist(string soundId)
        {
            return Json(_voiceFileService.GetSoundDeployHist(soundId));
        }
        #endregion
        #region Create - Update - Delete
        [HttpPost]
        public async Task<Result> DeployAudio(string soundNo) 
        {
            var audio = _voiceFileService.GetListAudioFile(soundNo, null).SingleOrDefault();
            HttpResponseMessage response;

            List<DeployDto> audios = new List<DeployDto>();
            if (audio == null)
                return new Result { Data = "", Message = "Deploy failed! not exist audio file on database", Success = false };

            try
            {
                audios.Add(new DeployDto { soundNo = soundNo, source = System.IO.File.ReadAllBytes(audio.localFileLocation), soundType=audio.soundType, extension=Path.GetExtension(audio.localFileLocation)});
                var data = new
                {
                    signature = 100,
                    frameID = 0,
                    functionCode = 8241,
                    dataLength = 0,
                    data = new
                    {
                        Data = audios
                    }
                };
                var result = _voiceFileService.UpdateVersionAudio(soundNo);
                response = await client.PostAsJsonAsync("http://26.115.12.45:81/Kiosk/KioskService/GetData", data);
                response.EnsureSuccessStatusCode();
                return new Result { Data = "", Message = "", Success = true };
            }
            catch
            {
                return new Result { Data = "", Message = "Deploy failed! not found audio file on server", Success = false };
            }
        }
        [HttpPost]
        public async Task<Result> DeployAudiosNew(string soundDtoJson)
        {
            HttpResponseMessage response;
            List<SoundDto> soundDtos = JsonConvert.DeserializeObject<List<SoundDto>>(soundDtoJson);
            List<string> errorAudio = new List<string>();
            List<DeployDto> audios = new List<DeployDto>();
            List<string> soundNos = new List<string>();
            soundDtos.ForEach(s =>
            {
                try
                {
                    audios.Add(new DeployDto { soundNo = s.soundNo, source = System.IO.File.ReadAllBytes(s.localFileLocation), soundType=s.soundType, extension = Path.GetExtension(s.localFileLocation) });
                    soundNos.Add(s.soundNo);
                }
                catch
                {
                    errorAudio.Add(s.soundName);
                }
            });

            if (errorAudio.Count > 0)
            {
                string mesError = "Not found";
                errorAudio.ForEach(e =>
                {
                    mesError += " [" + e + "]";
                });
                return new Result { Data = "", Message = mesError + " on the Server, please upload file before deploy!", Success = false };

            }
            else
            {
                var data = new
                {
                    signature = 100,
                    frameID = 0,
                    functionCode = 8241,
                    dataLength = 0,
                    data = new
                    {
                        Data = audios
                    }
                };
                soundNos.ForEach(sound =>
                {
                    var result = _voiceFileService.UpdateVersionAudio(sound);

                });
                try
                {
                    response = await client.PostAsJsonAsync("http://26.115.12.45:81/Kiosk/KioskService/GetData", data);
                    response.EnsureSuccessStatusCode();
                    return new Result { Data = "", Message = "", Success = true };
                }
                catch
                {
                    return new Result { Data = "", Message = "Can not connect to server Kiosk! Please try again", Success = false };
                }

            }
        }

        [HttpPost]
        public ActionResult<Result> DeployAudios(string soundDtoJson)
        {
            List<SoundDto> soundDtos = JsonConvert.DeserializeObject<List<SoundDto>>(soundDtoJson);
            List<string> errorAudio = new List<string>(); 
            List<byte[]> listAudioByte = new List<byte[]>();
            List<string> soundNos = new List<string>();
            soundDtos.ForEach(s =>
            {
                try
                {
                    listAudioByte.Add(System.IO.File.ReadAllBytes(s.localFileLocation));
                    soundNos.Add(s.soundNo);
                }
                catch
                {
                    errorAudio.Add(s.soundName);
                }
            });
            if (errorAudio.Count > 0)
            {
                string mesError = "Not found";
                errorAudio.ForEach(e =>
                {
                    mesError += " [" + e + "]";
                });

                return Json(new { Data = "", Message = mesError + " on the Server, please upload file before deploy!", Success = false });
            }
            else
            {
                for (int i = 0; i < soundNos.Count; i++)
                {
                    var result = _voiceFileService.SaveTempDeploy("1", "1", soundNos[i], listAudioByte[i]);
                    if(!result.Success) return Json(new { Data = "", Message = "Deploy failed!", Success = false });
                }
                var data = _voiceFileService.UpdateTempDeploy(soundNos[0]);
                return Json(new { Data = "", Message = "", Success = true });
            }
        }
        [HttpPost]
        public ActionResult<Result> SaveAudioFile(KIO_ClientSoundMgt clientSoundMgt)
        {
            return Json(_voiceFileService.SaveAudioFile(clientSoundMgt, CurrentUser.UserID));
        }
        [HttpDelete]
        public ActionResult<Result> DeleteAudioFile(string soundId)
        {
            try
            {
                var sound = _voiceFileService.DeleteAudioFile(soundId);
                if (sound.Success && System.IO.File.Exists(sound.Data.ToString()))
                {
                    try
                    {
                        System.IO.File.Delete(sound.Data.ToString());
                    }
                    catch
                    {
                        return Json(sound);
                    }
                }
                return Json(sound);
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0015 };
            }
        }
        [HttpDelete]
        public ActionResult<Result> DeleteListAudioFile(string soundIds)
        {

            try
            {
                List<SoundNoDto> sounds = JsonConvert.DeserializeObject<List<SoundNoDto>>(soundIds);
                List<string> fail = new List<string>();
                sounds.ForEach(s =>
                {
                    var result = _voiceFileService.DeleteAudioFile(s.soundNo);
                    if (result.Success && System.IO.File.Exists(result.Data.ToString()))
                    {
                        try
                        {
                            System.IO.File.Delete(result.Data.ToString());
                        }
                        catch
                        {
                            
                        }
                    }
                    if(!result.Success)
                    {

                        fail.Add(s.soundNo);
                    }
                });

                if(fail.Count > 0)
                {
                    string mesError = "Detele store ";
                    fail.ForEach(e =>
                    {
                        mesError += " [" + e + "]";
                    });
                    return new Result { Success = false, Message = mesError + " failure" };
                }
                return new Result { Success = true, Message = "Delete data of audio successfully." };
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0015 };
            }
        }
        #endregion
        #region Upload File
  
        [HttpPost]
        public ActionResult UploadAudioFile(List<IFormFile> files) 
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads/audios"); //we are using Temp file name just for the example. Add your own file path.
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePaths });
        }
        [HttpPost]
        public ActionResult<object> UploadAudio()
        {
            try
            {
                var myFile = Request.Form.Files["UploadFileAudio"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads/audios/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                {
                    myFile.CopyTo(fileStream);
                }
                
                //var outPath = Path.Combine(path, Path.GetFileNameWithoutExtension(myFile.FileName) + ".wav");

                //using (var reader = new MediaFoundationReader(Path.Combine(path, myFile.FileName)))
                //{
                //    WaveFileWriter.CreateWaveFile(outPath, reader) ;
                //}

                string base64audio = Convert.ToBase64String(System.IO.File.ReadAllBytes(Path.Combine(path, myFile.FileName)));
                return Json(new { data = "data:audio/wav;base64," + base64audio, path = Path.Combine(path, myFile.FileName) });
            }
            catch
            {
                Response.StatusCode = 400;
                return new EmptyResult();
            }
        }
        [HttpGet]
        public ActionResult<Result> PlayAudio(string path)
        {
            try
            {
                string base64audio = Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
                return Json(new Result { Data = "data:audio/wav;base64," + base64audio, Success = true, Message = "" });

            }
            catch
            {
                return Json(new Result { Data = null, Success = false, Message = "" });
            }
        }
        //public IActionResult OpenFileInBrowser(string fileguid)
        //{

        //    if (fileguid == null)
        //        return Content("file name not present");

        //    var fileMaster = filesService.GetSYFileUploadMasterByFileGuid(fileguid);
        //    var file = filesService.GetSYFileUploadByID(fileguid);
        //    var path = Path.Combine(fileMaster.FilePath, file.FileNameSave);
        //    var memory = new MemoryStream();

        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return new FileStreamResult(memory, "application/pdf");

        //}

        #endregion
    }
}
