using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileUpload.Models
{
    public class FileInforbyID
    {
        public string ID { get; set; }
        public string FileMasterID { get; set; }
        public string UrlPath { get; set; }
        public string Pag_ID { get; set; }
        public string Sp_Name { get; set; }
        public string Pag_Name { get; set; }
        public string Form_Name { get; set; }
        public string[] extensions { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Action_ReloadListFile { get; set; }
        public string Controller_ReloadListFile { get; set; }
        public string Sp_Name_CheckDeleteFile { get; set; }
        public string FileID { get; set; }     
        public int Site_ID { get; set; }

    }
}
