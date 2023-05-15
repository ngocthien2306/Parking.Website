using System;
using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using Modules.Pleiger.SalesProject.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.Inventory.Services.IService;
using Modules.Pleiger.Production.Services.IService;
using Modules.Pleiger.CommonModels;
using AutoMapper;
using Modules.Pleiger.Production.Services.ServiceImp;
using InfrastructureCore.Web.Extensions;
using System.Data;
using System.Reflection;
using System.IO;
using InfrastructureCore.Utils;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESProductStatusController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESProductStatusService _productStatusService;
        private readonly IUserService _userService;


        public MESProductStatusController(IHttpContextAccessor contextAccessor,IMESProductStatusService productStatusService, IUserService userService) : base(contextAccessor)
        {
            this._productStatusService = productStatusService;
            _userService = userService;
            _contextAccessor = contextAccessor;

        }

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;

            return View();

        }
        public object LoadProductStatus(DataSourceLoadOptions sourceLoadOptions, MES_ProductStatus productStatus)
        {
            var result = _productStatusService.GetProductStatus(sourceLoadOptions, productStatus);
            return DataSourceLoader.Load(result, sourceLoadOptions);
        }
        public IActionResult DownloadSalesInformation(string listSelected)
        {
            var result = _productStatusService.GetDataExportExcelICube(listSelected);
            string testJson = JsonConvert.SerializeObject(result);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(testJson);
            Type myType = typeof(MES_ProductStatus);
            MemberInfo[] myMembers = myType.GetMembers();
            for (int i = 0; i < myMembers.Length; i++)
            {
                if (myMembers[i].CustomAttributes.Any() == true)
                {
                    foreach (var item in myMembers[i].CustomAttributes)
                    {
                        if (item.AttributeType.Name == "ColumNameAttribute")
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName == myMembers[i].Name)
                                {
                                    col.Caption = item.ConstructorArguments[0].Value.ToString();
                                    //col.ColumnName = item.ConstructorArguments[0].Value.ToString();
                                }
                            }
                        }
                    }
                }
            }
            string downloadExcelPath = _productStatusService.ExportExcelICube(dt);
            var memory = new MemoryStream();
            using (var stream = new FileStream(downloadExcelPath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            var file = File(memory, ExcelExport.GetContentType(downloadExcelPath), downloadExcelPath.Remove(0, downloadExcelPath.LastIndexOf("/") + 1));
            return Json(new { Result = true, downloadExcelPath = downloadExcelPath, fileName = file.FileDownloadName });
        }
    }
}
