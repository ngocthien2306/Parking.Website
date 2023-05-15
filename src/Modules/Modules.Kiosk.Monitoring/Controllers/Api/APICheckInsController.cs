using InfrastructureCore.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Services.IService;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Kiosk.Monitoring.Repositories.Repository;
using Modules.Pleiger.CommonModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Web.Http;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using InfrastructureCore.Web.Controllers;

namespace Modules.Kiosk.Monitoring.Controllers.Api
{
    public class APICheckInsController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAccessMenuService _accessMenuService;


        private readonly IKIOCheckInRepository _checkInRepository;

        public APICheckInsController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IKIOCheckInRepository checkInRepository)
        {
            _contextAccessor = contextAccessor;
            _accessMenuService = accessMenuService;
            _checkInRepository = checkInRepository;
        }

        [System.Web.Http.HttpGet]
        public IActionResult GetImageTaken(string userId)
        {
            try
            { 
                var checkins = _checkInRepository.GetPhotoById(userId);
                return base.File(checkins.takenPhoto, "image/jpeg");

                //MemoryStream ms = new MemoryStream(checkins.takenPhoto);
                //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                //response.Content = new StreamContent(ms);
                //response.Content.Headers.ContentType = new
                //System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                ////send response of image/png type
                //return response;
            }

            catch
            {
                return null;
            }
        }
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetImageCardId(string userId)
        {
            try
            {
                var checkins = _checkInRepository.GetPhotoById(userId);
                MemoryStream ms = new MemoryStream(checkins.idCardPhoto);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new
                System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                //send response of image/png type
                return response;
            }
            catch
            {
                return null;
            }

        }
    }
}
