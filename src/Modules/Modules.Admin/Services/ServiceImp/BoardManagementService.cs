using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Admin.Services.ServiceImp
{
    public class BoardManagementService : IBoardManagementService
    {
        private const string SP_Name = "SP_Web_BoardManagement";
        private const string SP_Name_CB = "SP_Web_CommonBoardManagement";
        #region Get BoardIfo
        public List<SYBoardInfo> GetListBoardInfo()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetBoard";
                var result = conn.ExecuteQuery<SYBoardInfo>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        public SYBoardInfo GetBoardInfo(string BoardID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetailBoard";
                arrParamsValue[1] = BoardID;
                var result = conn.ExecuteQuery<SYBoardInfo>(SP_Name, arrParams, arrParamsValue);
                return result.FirstOrDefault();
            }
        }
        // Quan  add siteID 2021-01-15
        public Result SaveBoardInfo(SYBoardInfo boardInfo, int siteID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[14];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@GroupID";
                    arrParams[3] = "@Creator";
                    arrParams[4] = "@CreatorID";
                    arrParams[5] = "@BoardName";
                    arrParams[6] = "@BoardTitle";
                    arrParams[7] = "@BoardStatus";
                    arrParams[8] = "@BoardSkin";
                    arrParams[9] = "@BoardType";
                    arrParams[10] = "@BoardDesc";
                    arrParams[11] = "@Owner";
                    arrParams[12] = "@OwnerID";
                    arrParams[13] = "@SiteID";
                    object[] arrParamsValue = new object[14];
                    arrParamsValue[0] = "SaveBoard";
                    arrParamsValue[1] = boardInfo.BoardID;
                    arrParamsValue[2] = 0;
                    arrParamsValue[3] = boardInfo.Creator;
                    arrParamsValue[4] = boardInfo.CreatorID;
                    arrParamsValue[5] = boardInfo.BoardName;
                    arrParamsValue[6] = boardInfo.BoardTitle;
                    arrParamsValue[7] = boardInfo.BoardStatus;
                    arrParamsValue[8] = boardInfo.BoardSkin ?? "";
                    arrParamsValue[9] = boardInfo.BoardType;
                    arrParamsValue[10] = boardInfo.BoardDesc;
                    arrParamsValue[11] = boardInfo.Owner ?? "";
                    arrParamsValue[12] = boardInfo.OwnerID;
                    arrParamsValue[13] = siteID;
                    //SP_Web_BoardManagement
                    var rs = conn.ExecuteNonQuery(SP_Name, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        #endregion
        #region Delete board management
        public Result DeleteBoardInfo(List<SYBoardInfo> ListBoardInfo)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in ListBoardInfo)
                        {
                            string[] arrParams = new string[2];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@BoardID";
                            object[] arrParamsValue = new object[2];
                            arrParamsValue[0] = "OnDelete";
                            arrParamsValue[1] = item.BoardID;
                            var rs = conn.ExecuteNonQuery(SP_Name, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }
                        result.Success = true;
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Data = ex;
                    }

                }
            }

            return result;
        }
        #endregion
        #region GetListBoardBranch
        public List<SYBoardBranchNames> GetListBoardBranchByBoardID(string boardID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListBoardBranchByBoardID";
                arrParamsValue[1] = boardID;
                var result = conn.ExecuteQuery<SYBoardBranchNames>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        #endregion
        #region Save Common Board
        public Result SaveCommonBoardInfo(SYBoardContent boardInfo, string action)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[21];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@ProMode";
                    arrParams[2] = "@BoardID";
                    arrParams[3] = "@BoardDocID";
                    arrParams[4] = "@InsertName";
                    arrParams[5] = "@InsertID";
                    arrParams[6] = "@InternalPhone";
                    arrParams[7] = "@Email";
                    arrParams[8] = "@Subject";
                    arrParams[9] = "@FileID";
                    arrParams[10] = "@ReplyNum";
                    arrParams[11] = "@ReplyDepth";
                    arrParams[12] = "@DocStatus";
                    arrParams[13] = "@Body";
                    arrParams[14] = "@NoticeDT";
                    arrParams[15] = "@NoticeOption";
                    arrParams[16] = "@ReferenceUrl";
                    arrParams[17] = "@DocDT";
                    arrParams[18] = "@BranchName";
                    arrParams[19] = "@NoticeTime";
                    arrParams[20] = "@ParentReplyNum";
                    // arrParams[19] = "@DocAuth";
                    object[] arrParamsValue = new object[21];
                    arrParamsValue[0] = "SaveCommonBoard";
                    arrParamsValue[1] = action;
                    arrParamsValue[2] = boardInfo.BoardID;
                    arrParamsValue[3] = boardInfo.BoardDocID;
                    arrParamsValue[4] = boardInfo.InsertName;
                    arrParamsValue[5] = boardInfo.InsertID;
                    arrParamsValue[6] = boardInfo.InternalPhone;
                    arrParamsValue[7] = boardInfo.Email;
                    arrParamsValue[8] = boardInfo.Subject;
                    arrParamsValue[9] = boardInfo.FileID;
                    arrParamsValue[10] = boardInfo.ReplyNum;
                    arrParamsValue[11] = boardInfo.ReplyDepth;
                    arrParamsValue[12] = boardInfo.DocStatus;
                    arrParamsValue[13] = boardInfo.Body;
                    arrParamsValue[14] = boardInfo.DocDT.AddDays(boardInfo.NoticeTime);
                    arrParamsValue[15] = boardInfo.NoticeOption;
                    arrParamsValue[16] = boardInfo.ReferenceUrl;
                    arrParamsValue[17] = boardInfo.DocDT;
                    arrParamsValue[18] = boardInfo.BranchName;
                    arrParamsValue[19] = boardInfo.NoticeTime;
                    arrParamsValue[20] = boardInfo.ParentReplyNum;
                    //  arrParamsValue[19] = boardInfo.DocAuth;
                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        #endregion
        #region Delete Common Board
        public Result DeleteCommonBoardInfo(string BoardID, int BoardDocID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BoardDocID";
                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "DeleteCommonBoard";
                    arrParamsValue[1] = BoardID;
                    arrParamsValue[2] = BoardDocID;
                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }

        public Result DeleteBoardContent(List<SYBoardContent> ListBoardContentInfo)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in ListBoardContentInfo)
                        {
                            string[] arrParams = new string[3];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@BoardID";
                            arrParams[2] = "@BoardDocID";
                            object[] arrParamsValue = new object[3];
                            arrParamsValue[0] = "DeleteCommonBoard";
                            arrParamsValue[1] = item.BoardID;
                            arrParamsValue[2] = item.BoardDocID;
                            var rs = conn.ExecuteNonQuery(SP_Name_CB, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }
                        result.Success = true;
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Data = ex;
                    }
                }
            }
            return result;
        }


        #endregion
        #region ReadCount
        public Result ReadCountCommonBoard(string BoardID, int BoardDocID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BoardDocID";
                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "ReadCountCommonBoard";
                    arrParamsValue[1] = BoardID;
                    arrParamsValue[2] = BoardDocID;
                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        #endregion
        #region Get List Common Board
        public List<SYBoardContent> GetListCommonBoardByBoardID(string boardID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListCommonBoard";
                arrParamsValue[1] = boardID;
                var result = conn.ExecuteQuery<SYBoardContent>(SP_Name_CB, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        #endregion
        public List<SYBoardContent> GetListNoticeInBoard(string BoardID, string RowNumberDisplay, int BranchName, List<SYMenu> listMenuNotice)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string MenuNociceID1 = "";
                string MenuNociceID2 = "";
                if(listMenuNotice.Count>0)
                {
                    MenuNociceID1 = listMenuNotice[0].MenuPath.Substring(8);
                    if (listMenuNotice.Count>1)
                    {
                        MenuNociceID2= listMenuNotice[1].MenuPath.Substring(8);
                    }    
                }    
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                arrParams[2] = "@BoardID1";
                arrParams[3] = "@BoardID2";
                //arrParams[2] = "@RowNumberDisplay";
                //arrParams[3] = "@BranchName";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetListNoticeInBoard";
                arrParamsValue[1] = BoardID;
                arrParamsValue[2] = MenuNociceID1;
                arrParamsValue[3] = MenuNociceID2;
                //arrParamsValue[2] = RowNumberDisplay;
                //arrParamsValue[3] = BranchName;
                //SP_Web_CommonBoardManagement
                var result = conn.ExecuteQuery<SYBoardContent>(SP_Name_CB, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        #region #region Get Common Board by ID
        public SYBoardContent GetCommonBoardByBoardID(string boardID, int BoardDocID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                arrParams[2] = "@BoardDocID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetCommonBoard";
                arrParamsValue[1] = boardID;
                arrParamsValue[2] = BoardDocID;
                var result = conn.ExecuteQuery<SYBoardContent>(SP_Name_CB, arrParams, arrParamsValue);
                return result.FirstOrDefault();
            }
        }
        public SYBoardContent GetListBoardContent(int BoardDocID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";               
                arrParams[1] = "@BoardDocID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListBoardContent";           
                arrParamsValue[1] = BoardDocID;
                var result = conn.ExecuteQuery<SYBoardContent>(SP_Name_CB, arrParams, arrParamsValue);
                return result.FirstOrDefault();
            }
        }     

        #endregion
        #region board branch
        public Result SaveBoardBranch(string boardID, string branchName)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BranchNameDesc";
                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "InsertBoardBranch";
                    arrParamsValue[1] = boardID;
                    arrParamsValue[2] = branchName;
                    var rs = conn.ExecuteNonQuery(SP_Name, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        public Result DeleteBoardBranch(string boardID, int branchNameID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BranchNameID";
                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "DeleteBoardBranch";
                    arrParamsValue[1] = boardID;
                    arrParamsValue[2] = branchNameID;
                    var rs = conn.ExecuteNonQuery(SP_Name, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        public List<SYBoardBranchNames> GetListBoardBranchByID(int bid)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                arrParams[0] = "@BoardID";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "ListBoardBranchByID";
                arrParamsValue[0] = bid;
                var result = conn.ExecuteQuery<SYBoardBranchNames>(SP_Name, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        #endregion
        #region Comment board
        public Result SaveBoardComment(SYBoardComment data)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[5];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BoardDocID";
                    arrParams[3] = "@CommentBody";
                    arrParams[4] = "@InsertID";
                    object[] arrParamsValue = new object[5];
                    arrParamsValue[0] = "InsertComment";
                    arrParamsValue[1] = data.BoardID;
                    arrParamsValue[2] = data.BoardDocID;
                    arrParamsValue[3] = data.CommentBody;
                    arrParamsValue[4] = data.InsertID;
                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        public Result DeleteBoardComment(int CommentID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@CommentID";
                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "DeleteComment";
                    arrParamsValue[1] = CommentID;

                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        public List<SYBoardComment> GetCommentBoardByBoardID(string boardID, int BoardDocID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@BoardID";
                arrParams[2] = "@BoardDocID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListComment";
                arrParamsValue[1] = boardID;
                arrParamsValue[2] = BoardDocID;
                var result = conn.ExecuteQuery<SYBoardComment>(SP_Name_CB, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        #endregion
        #region Select board content confirm
        public List<SYBoardContent> GetListBoardContentConfirm(string UserID, List<SYMenu> listMenuNotice)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                // Quan add 2021-01-15
                string MenuNociceID1 = "";
                string MenuNociceID2 = "";
              
                if (listMenuNotice.Count > 0)
                {
                    MenuNociceID1 = listMenuNotice[0].MenuPath.Substring(8);
                    if (listMenuNotice.Count > 1)
                    {
                        MenuNociceID2 = listMenuNotice[1].MenuPath.Substring(8);
                    }
                }
                
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@UserID";
                arrParams[2] = "@BoardID1"; // Quan add 2021-01-15
                arrParams[3] = "@BoardID2"; // Quan add 2021-01-15
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetBoardContentConfirm";
                arrParamsValue[1] = UserID;
                arrParamsValue[2] = MenuNociceID1; // Quan add 2021-01-15
                arrParamsValue[3] = MenuNociceID2; // Quan add 2021-01-15
                var result = conn.ExecuteQuery<SYBoardContent>(SP_Name_CB, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        #endregion
        #region Insert board content confirm
        public Result InsertBoardContentConfirm(string BoardID, int BoardDocID, string UserID)
        {
            Result result = new Result();
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[4];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@BoardID";
                    arrParams[2] = "@BoardDocID";
                    arrParams[3] = "@UserID";
                    object[] arrParamsValue = new object[4];
                    arrParamsValue[0] = "InsertBoardContentConfirm";
                    arrParamsValue[1] = BoardID;
                    arrParamsValue[2] = BoardDocID;
                    arrParamsValue[3] = UserID;
                    var rs = conn.ExecuteNonQuery(SP_Name_CB, arrParams, arrParamsValue);
                    result.Success = true;
                    result.Data = rs;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = ex;
            }
            return result;
        }
        #endregion
    }
}
