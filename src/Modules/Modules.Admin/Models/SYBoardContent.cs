using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class SYBoardContent
    {
        public string BoardID { get; set; }
        public int BoardDocID { get; set; }
        public string InsertID { get; set; }
        public string InsertName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string InternalPhone { get; set; }
        public DateTime? InsertDT { get; set; }
        public string InsertDTNew { get; set; }
        public DateTime? UpdateDT { get; set; }
        public int ReadCount { get; set; }
        public int ReplyNum { get; set; }
        public int ReplyDepth { get; set; }
        public string DocStatus { get; set; }
        public string Body { get; set; }
        public string NoticeOption { get; set; }
        public string OwnerName { get; set; }        
        public DateTime NoticeDT { get; set; }
        public DateTime DocDT { get; set; }
        public string ReferenceUrl { get; set; }
        public string FileID { get; set; }
        public int BranchName { get; set; }
        public int BoardDocKey { get; set; }
        //public string DocAuth { get; set; }
        public int ParentReplyNum { get; set; }
        public int SortOrder { get; set; }
        public int NoticeOrder { get; set; }
        public int NoticeTime { get; set; }
        
        public List<SYBoardComment> lstComment { get; set; }
        public SYBoardContent prevCB { get; set; }
        public SYBoardContent nextCB { get; set; }
    }
}
