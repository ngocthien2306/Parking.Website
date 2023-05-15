using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Authorization;
using InfrastructureCore.Models.Module;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using System.Collections.Generic;

namespace Modules.Admin.Controllers
{
    public class ModulesController : Controller
    {
        #region Properties
        private IModulesService _modulesService;
        #endregion

        #region Constructor
        public ModulesController(IModulesService modulesService)
        {
            this._modulesService = modulesService;
        }
        #endregion

        #region Modules Management By Site

        [CustomAuthorization]
        public IActionResult ModulesManagement()
        {
            return View();
        }

        [HttpGet]
        public object GetAllModulesBySite(DataSourceLoadOptions loadOptions)
        {
            List<SYModulesMg> lstData = _modulesService.GetAllModulesBySite();
            return DataSourceLoader.Load(lstData, loadOptions);
        }

        [HttpPost]
        public IActionResult SavedDataModulesBySite(SYModulesMg data, string state)
        {
            Result result = _modulesService.SavedDataModulesBySite(data, state);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult DeletedDataModulesBySite(SYModulesMg data)
        {
            Result result = _modulesService.DeletedDataModulesBySite(data);
            return Json(new { result.Success, result.Message });
        }

        [HttpPost]
        public IActionResult ApplyModulesConfig()
        {
            Result result = _modulesService.ApplyModulesConfig();
            return Json(new { result.Success, result.Message });
        }


        #endregion
    }
}
