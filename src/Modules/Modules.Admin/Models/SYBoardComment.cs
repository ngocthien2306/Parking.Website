using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class SYBoardComment
    {
        public int CommentID { get; set; }
        public string BoardID { get; set; }
        public int BoardDocID { get; set; }
        public string CommentBody { get; set; }
        public string InsertID { get; set; }
        public DateTime InsertDT { get; set; }
        public int SortOrder { get; set; }
    }
}
