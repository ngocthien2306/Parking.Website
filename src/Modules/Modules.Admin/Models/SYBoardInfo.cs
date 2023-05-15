using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class SYBoardInfo
    {
        public string BoardID { get; set; }
        public string BoardName { get; set; }
        public string BoardType { get; set; }
        public string BoardTitle { get; set; }
        public string Owner { get; set; }
        public string OwnerID { get; set; }
        public string Creator { get; set; }
        public string CreatorID { get; set; }
        public DateTime InsertDT { get; set; }
        public DateTime UpdateDT { get; set; }
        public DateTime DeleteDT { get; set; }
        public string BoardStatus { get; set; }
        public string BoardDesc { get; set; }
        public string BoardSkin { get; set; }

        // Quan add file 2020/10/13
        public bool Upload_File { get; set; }
        public bool Delele_File { get; set; }
        public string ID { get; set; }
        public string FileMasterID { get; set; }
        public string FileID { get; set; }
        public string UrlPath { get; set; }
        public string Pag_ID { get; set; }
        public string Sp_Name { get; set; }
        public string Pag_Name { get; set; }
        public string Form_Name { get; set; }
        public string Action_ReloadListFile { get; set; }
        public string Controller_ReloadListFile { get; set; }
    }
}
