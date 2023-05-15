using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
//using Modules.Pleiger.Models;
//using Modules.Pleiger.Services.IService;


namespace Modules.Admin.Controllers
{
    public class CBController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IBoardManagementService boardManagementService;
        private readonly IMenuService menuService;
        private readonly IGroupUserService _groupUserService;
        private readonly IAccessMenuService _accessMenuService;
        //private readonly IPurchaseService _purchaseService;

        public CBController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IGroupUserService groupUserService, IBoardManagementService boardManagementService, IMenuService menuService) : base(contextAccessor)
        {
            
     
            this.contextAccessor = contextAccessor;
            this.boardManagementService = boardManagementService;
            this.menuService = menuService;
            this._groupUserService = groupUserService;
            this._accessMenuService = accessMenuService;
            //_purchaseService = purchaseService;
        }

        public IActionResult Index(string bid)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if(CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.SiteID = CurrentUser.SiteID;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserType = CurrentUser.UserType;

            var boardInfo = boardManagementService.GetBoardInfo(bid);
            var lstBranch = boardManagementService.GetListBoardBranchByBoardID(bid);
            ViewBag.lstBranch = lstBranch;   

            //check menu
            string strUrl = "/CB?bid=" + bid;
            var menu = menuService.GetListDataByGroup(CurrentUser.SiteID).Where(m => m.MenuPath == strUrl).FirstOrDefault();
            ViewBag.IsBack = menu == null ? true : false;
            return View(boardInfo);
        }
        public IActionResult Detail(string bid, int bdid, string ViewBagIndex,int MenuIDParent)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.SiteID = CurrentUser.SiteID;
            if (MenuIDParent != 0)
            {                
                // Quan add 2020/11/23
                // Get User permission file Upload by GruopUser      
                var listSumFileUploadByMenuID = _accessMenuService.SelectSumFileUploadByMenuID(MenuIDParent, CurrentUser.UserID);
                var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(MenuIDParent, CurrentUser.UserID);
                // check user In Group Permission
                if (listSumFileUploadByMenuID.Count > 0)
                {   
                    if (listSumFileUploadByMenuID[0].UPLOAD_FILE_SUM > 0)
                    {
                        ViewBag.Upload_File = true;
                    }
                    else
                    {
                        ViewBag.Upload_File = false;

                    }
                    if (listSumFileUploadByMenuID[0].DELETE_FILE_SUM > 0)
                    {
                        ViewBag.Delele_File = true;
                    }
                    else
                    {
                        ViewBag.Delele_File = false;

                    }
                }
                // check user Permission
                if (listUserPermissionAccessMenu.Count > 0)
                {
                    if (listUserPermissionAccessMenu[0].UPLOAD_FILE_YN == true)
                    {
                        ViewBag.Upload_File = true;
                    }
                    else
                    {
                        ViewBag.Upload_File = false;

                    }
                    if (listUserPermissionAccessMenu[0].DELETE_FILE_YN == true)
                    {
                        ViewBag.Delele_File = true;
                    }
                    else
                    {
                        ViewBag.Delele_File = false;

                    }
                }              
            }
            else
            {
                ViewBag.Upload_File = false;
                ViewBag.Delele_File = false;
            }

            ViewBag.MenuId = MenuIDParent;
            ViewBag.BID = bid;
            ViewBag.BDID = bdid;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserID = CurrentUser.UserID;
            ViewBag.UserType = CurrentUser.UserType;
            ViewBag.ViewBagIndex = ViewBagIndex;

            var boardInfo = boardManagementService.GetBoardInfo(bid);
            ViewBag.OwnerID = boardInfo.OwnerID;
            bool isEdit = false;
            if(CurrentUser.UserName == boardInfo.CreatorID)
            {
                isEdit = true;
            }
            ViewBag.IsEdit = isEdit;

            // Quan add change file 2020/10/13
            boardInfo.ID = "FileID" + ViewBag.Thread;
            boardInfo.UrlPath = "";
            //boardInfo.Pag_ID = projectCode;
            boardInfo.FileMasterID = boardInfo.FileID;
            boardInfo.Pag_Name = "";
            string bdid1 = bdid.ToString();
            boardInfo.Pag_ID = bdid1;
           
            // Hiện tại đang upload file bằng boardID          
            // Thay đổi thành upload file cho từng BoardDocID

            //if (boardInfo.FileID == null || boardInfo.FileID == "")
            //{
            //    boardInfo.FileMasterID = Guid.NewGuid().ToString();
            //    boardInfo.FileID = boardInfo.FileMasterID;
            //}

            // Quan add change file 2020/10/29
            // Insert, Update new file in CommonBoard
            var GetCommonBoardInfo = boardManagementService.GetCommonBoardByBoardID(bid, bdid);

            if (GetCommonBoardInfo!=null)
            {
                if (GetCommonBoardInfo.FileID == null|| GetCommonBoardInfo.FileID == "")
                {
                    //boardInfo.Pag_ID = Guid.NewGuid().ToString();
                    boardInfo.FileID = Guid.NewGuid().ToString();
                }
                else
                {
                    boardInfo.FileID = GetCommonBoardInfo.FileID;
                   // boardInfo.Pag_ID = GetCommonBoardInfo.FileID;

                }
            }    
            else
            {
                boardInfo.Pag_ID = Guid.NewGuid().ToString();
            }    
            return View(boardInfo);
        }
        [HttpGet]
        public IActionResult ReloadGridFile(string ID)
        {  
            var boardInfo = boardManagementService.GetBoardInfo(ID);           
            return Json(boardInfo);
        }
        // Quan add reload BoardContent
        public IActionResult GetListBoardContent(int ID)
        {
            var boardInfo = boardManagementService.GetListBoardContent(ID);
            return Json(boardInfo);
        }
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions, string bid)
        {
            var data = boardManagementService.GetListCommonBoardByBoardID(bid).OrderByDescending(m => m.InsertDT);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListNoticeInBoard(DataSourceLoadOptions loadOptions, string BoardID, string RowNumberDisplay,int BranchName)
        {
            // Quan add
            var listMenuAuthorized = CurrentUser.AuthorizedMenus;
            var listMenuNotice = new List<SYMenu>();

            if (listMenuAuthorized.Count > 0)
            {
                listMenuNotice = listMenuAuthorized.Where(x => x.MenuPath.Contains("/CB?bid=") && x.MenuLevel == 2 && x.SiteID == CurrentUser.SiteID).ToList();
            }

            //var data = boardManagementService.GetListNoticeInBoard(BoardID, RowNumberDisplay, BranchName,listMenuNotice);
            var data = boardManagementService.GetListNoticeInBoard(BoardID, RowNumberDisplay, BranchName,listMenuNotice);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        
        [HttpPost]
        public IActionResult GetCommonBoardInfo(string BoardID,int BoardDocID)
        {
            var result = boardManagementService.GetCommonBoardByBoardID(BoardID,BoardDocID);
            if(result != null)
            {
                var commentRs = boardManagementService.GetCommentBoardByBoardID(BoardID, BoardDocID);
                result.lstComment = commentRs;
            } 
            else
            {
                return Json(new { Success = true, Data = result });

            }

            var data = boardManagementService.GetListCommonBoardByBoardID(BoardID);

            //get next prev cb
            SYBoardContent prev = new SYBoardContent();
            SYBoardContent next = new SYBoardContent();
            List<SYBoardContent> list = new List<SYBoardContent>();
           
            list = data.Where(m => m.ParentReplyNum == result.ParentReplyNum).ToList();

            for (int i = 0; i< list.Count;i++)
            {
                if(list[i].BoardDocID == result.BoardDocID)
                {
                    if(list.Count > 2)
                    {
                        if (i == 0)
                        {
                            next = list[i + 1];
                            prev = new SYBoardContent();
                        }
                        else if (i == list.Count - 1)
                        {
                            prev = list[i - 1];
                            next = new SYBoardContent();
                        }
                        else
                        {
                            prev = list[i - 1];
                            next = list[i + 1];
                        }
                    }                    
                }
            }
            result.prevCB = prev;
            result.nextCB = next;
            return Json(new { Success = true, Data = result });
        }
        [HttpGet]
        public IActionResult GetListBoardBranchByBoardID(DataSourceLoadOptions loadOptions, string bid)
        {
            var data = boardManagementService.GetListBoardBranchByBoardID(bid);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetRangeNoticeDate(DataSourceLoadOptions loadOptions, int boardID, string textDay)
        {
            List<BoardContentNoticeDate> nds = new List<BoardContentNoticeDate>();
            nds.Add(new BoardContentNoticeDate() { TextDay = 1 + " Day", Day = 1 });
            for (int i = 2; i <= 15; i++)
            nds.Add(new BoardContentNoticeDate() { TextDay = i +  " " + textDay, Day = i });
            nds.Add(new BoardContentNoticeDate() { TextDay = 20 + " "+ textDay, Day = 20 });
            nds.Add(new BoardContentNoticeDate() { TextDay = 30 + " "+ textDay, Day = 30 });
            var loadResult = DataSourceLoader.Load(nds, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpPost]
        public IActionResult OnSave(SYBoardContent CommonBoardInfo, string actionCB)
        {
            if(actionCB == "REPLY")
            {
                CommonBoardInfo.ParentReplyNum = CommonBoardInfo.BoardDocID;
            }
            CommonBoardInfo.InsertID = CurrentUser.UserID;
            var result = boardManagementService.SaveCommonBoardInfo(CommonBoardInfo, actionCB);
            return Json(new { result.Success, result.Message });
           // return Ok();
        }
        [HttpPost]
        public IActionResult OnDelete(string BoardID, int BoardDocID)
        {
            var result = boardManagementService.DeleteCommonBoardInfo(BoardID, BoardDocID);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        
        [HttpPost]
        public IActionResult OnInsertComment(SYBoardComment CommentCBInfo)
        {
            CommentCBInfo.InsertID = CurrentUser.UserName;
            var result = boardManagementService.SaveBoardComment(CommentCBInfo);
            var lstComment = boardManagementService.GetCommentBoardByBoardID(CommentCBInfo.BoardID,CommentCBInfo.BoardDocID);
            result.Data = lstComment;
            return Json(new { result.Success, result.Data });
        }
        [HttpPost]
        public IActionResult OnDeleteComment(int CommentID, string BoardID, int BoardDocID)
        {
            var result = boardManagementService.DeleteBoardComment(CommentID);
            var lstComment = boardManagementService.GetCommentBoardByBoardID(BoardID, BoardDocID);
            result.Data = lstComment;
            return Json(new { result.Success, result.Data });
        }
        [HttpPost]
        public IActionResult OnReadCount(string BoardID, int BoardDocID)
        {
            var result = boardManagementService.ReadCountCommonBoard(BoardID, BoardDocID);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnConfirmBoardContent(string BoardID, int BoardDocID)
        {
            var result = boardManagementService.InsertBoardContentConfirm(BoardID, BoardDocID,CurrentUser.UserID);
            return Json(new { result.Success, result.Message });
        }
    }
}
