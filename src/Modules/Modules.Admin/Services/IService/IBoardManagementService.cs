using InfrastructureCore;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Services.IService
{
    public interface IBoardManagementService
    {
        Result SaveBoardInfo(SYBoardInfo boardInfo, int siteID);
        List<SYBoardInfo> GetListBoardInfo();
        SYBoardInfo GetBoardInfo(string BoardID);
        Result DeleteBoardInfo(List<SYBoardInfo> ListBoardInfo);
        Result DeleteBoardContent(List<SYBoardContent> ListBoardContentInfo);        
        List<SYBoardBranchNames> GetListBoardBranchByBoardID(string boardID);
        Result SaveCommonBoardInfo(SYBoardContent boardInfo, string action);
        Result SaveBoardBranch(string boardID, string branchName);
        Result DeleteBoardBranch(string boardID, int branchNameID);
        List<SYBoardBranchNames> GetListBoardBranchByID(int bid);
        List<SYBoardContent> GetListCommonBoardByBoardID(string boardID);
        List<SYBoardContent> GetListNoticeInBoard(string BoardID, string RowNumberDisplay, int BranchName,List<SYMenu> listMenuNotice);
        //List<SYBoardContent> GetListNoticeInBoard(string BoardID, string RowNumberDisplay, int BranchName);
        SYBoardContent GetCommonBoardByBoardID(string boardID, int BoardDocID); 
        SYBoardContent GetListBoardContent(int BoardDocID);
        Result DeleteCommonBoardInfo(string BoardID, int BoardDocID);        
        Result SaveBoardComment(SYBoardComment data);
        Result DeleteBoardComment(int CommentID);
        List<SYBoardComment> GetCommentBoardByBoardID(string boardID, int BoardDocID);
        Result ReadCountCommonBoard(string BoardID, int BoardDocID);
        List<SYBoardContent> GetListBoardContentConfirm(string UserID,List<SYMenu> listMenuNotice);
        Result InsertBoardContentConfirm(string BoardID, int BoardDocID, string UserID);
    }
}
