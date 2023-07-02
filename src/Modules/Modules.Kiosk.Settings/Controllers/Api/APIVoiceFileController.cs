using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using InfrastructureCore;
using InfrastructureCore.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Kiosk.Monitoring.Repositories.Repository;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using Spire.Pdf.OPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages;

namespace Modules.Kiosk.Settings.Controllers.Api
{
    public class APIVoiceFileController : ApiController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUserService _userService;
        private readonly IUserManager _userManager;
        private readonly IKIOVoiceFileRepository _voiceFileService;


        public APIVoiceFileController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IUserService userService, 
            IUserManager userManager, IKIOVoiceFileRepository kiovoFileRepository)
        {
            _contextAccessor = contextAccessor;
            _accessMenuService = accessMenuService;
            _userService = userService;
            _userManager = userManager;
            _voiceFileService = kiovoFileRepository;
    
        }
        public IHttpActionResult Index()
        {
            return Ok();
        }
        [System.Web.Http.HttpPost]
        public List<KIO_ClientSoundMgt> GetListAudioFile(string audioId, string audioName)
        {
            return _voiceFileService.GetListAudioFile(audioId, audioName);
        }
        [System.Web.Http.HttpGet]
        public List<ResultAudioDto> GetSourceAudio(string strNo) 
        {
            List<ResultAudioDto> results = new List<ResultAudioDto>();

            try
            {
                List<SoundNoDto> soundNos = JsonConvert.DeserializeObject<List<SoundNoDto>>(strNo);
                soundNos.ForEach(s =>
                {
                    var audio = _voiceFileService.GetListAudioFile(s.soundNo, null).SingleOrDefault();

                    try
                    {
                        if (audio == null)
                        {
                            results.Add(new ResultAudioDto { Success = false, Message = "Not found audio with soundNo " + s.soundNo, Data = null, SoundName = "", SoundNo=s.soundNo}) ;
                        }
                        else
                        {
                            results.Add(new ResultAudioDto { Success = true, Message = "Download audio successfull", Data = System.IO.File.ReadAllBytes(audio.localFileLocation), SoundName = audio.soundName, SoundNo = audio.soundNo.ToString() });
                        }
                    }
                    catch
                    {
                        results.Add(new ResultAudioDto { Success = false, Message = "Not found file on server", Data = null, SoundName = audio.soundName, SoundNo = audio.soundNo.ToString() });
                    }
                });
                return results;
            }
            catch(Exception e)
            {
                results.Add(new ResultAudioDto { Success = false, Message = e.Message, Data = null });
            }
            return results;
        }
        [System.Web.Http.HttpGet]
        public ResultAudioDto GetSourceAudioFile(string soundNo)
        {
            ResultAudioDto results = new ResultAudioDto();
            try
            {
                var audio = _voiceFileService.GetListAudioFile(soundNo, null).SingleOrDefault();
                if (audio == null)
                {
                    results = (new ResultAudioDto { Success = false, Message = "Not found audio with soundNo " + soundNo, Data = null, SoundName = "", SoundNo = soundNo });
                }
                else
                {
                    results = (new ResultAudioDto { Success = true, Message = "Download audio successfull", Data = System.IO.File.ReadAllBytes(audio.localFileLocation), SoundName = audio.soundName, SoundNo = audio.soundNo.ToString() });
                }
                return results;
            }
            catch (Exception e)
            {
                results = (new ResultAudioDto { Success = false, Message = e.Message, Data = null });
            }
            return results;
        }
    }
}
