using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;

namespace Modules.Admin.Controllers
{
    public class BoardManagementController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IBoardManagementService boardManagementService;
        public BoardManagementController(IHttpContextAccessor contextAccessor, IBoardManagementService boardManagementService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.boardManagementService = boardManagementService;
        }

        public IActionResult Index()
        {
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            //Quan change 2020/10/27
            ViewBag.UserName = CurrentUser != null ? CurrentUser.UserName : null;
            ViewBag.UserID = CurrentUser != null ? CurrentUser.UserID : null;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            

            return View();
        }
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = boardManagementService.GetListBoardInfo();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpPost]
        // Save Board Managerment Infor 
        public IActionResult OnSave(SYBoardInfo boardInfo, string tempBoardID)
        {
            boardInfo.OwnerID = CurrentUser.UserID;   
            boardInfo.Owner = CurrentUser.UserName;
            boardInfo.Creator = CurrentUser.UserName;
            boardInfo.CreatorID = CurrentUser.UserID;
            var result = boardManagementService.SaveBoardInfo(boardInfo,CurrentUser.SiteID);
            //var result = boardManagementService.SaveBoardInfo(boardInfo);
            //if(result.Success == true)
            //{
            //    boardManagementService.UpdateBoardBranchWitTempID(tempBoardID, boardInfo.BoardID);
            //}
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnDelete(List<SYBoardInfo> listBoardInfo)
        {
             var result = boardManagementService.DeleteBoardInfo(listBoardInfo);
              return Json(new { result.Success, result.Message });
           // return Json("");
        }
        [HttpPost]
    
        public IActionResult DeleteBoardContent(List<SYBoardContent> ListBoardContentInfo)
        {
            var result = boardManagementService.DeleteBoardContent(ListBoardContentInfo);
            return Json(new { result.Success, result.Message });           
        }
        [HttpPost]
        public IActionResult GetBoardInfo(string BoardID)
        {
            var result = boardManagementService.GetBoardInfo(BoardID);
            //  var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Json(new { Success = true, Data = result });
           // return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListBoardBranchData(DataSourceLoadOptions loadOptions, string bid)
        {
            var data = boardManagementService.GetListBoardBranchByBoardID(bid);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpPost]
        public IActionResult OnSaveBranch(SYBoardBranchNames data, string bid)
        {
            var result = boardManagementService.SaveBoardBranch(bid, data.BranchNameDesc);
             return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnDeleteBranch(int BranchNameID, string bid)
        {
            var result = boardManagementService.DeleteBoardBranch(bid, BranchNameID);
            return Json(new { result.Success, result.Message });
        }
        [HttpGet]
        public IActionResult GenergateGUUID()
        {
            return Json(Guid.NewGuid().ToString("N"));
        }
    }
}
