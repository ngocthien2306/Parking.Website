using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureCore.Models.Identity
{
    public class SYUserGroups : IdentityRole<string>
    {
        public int NO { get; set; }
        public int GROUP_ID { get; set; }
        [Required]
        public string GROUP_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public int DEPT_CODE { get; set; }
        public int SITE_ID { get; set; }
        public bool SearchYN { get; set; }
        public bool CreateYN { get; set; }
        public bool SaveYN { get; set; }
        public bool DeleteYN { get; set; }
        public bool EditYN { get; set; }
        public bool ExcelYN { get; set; }
        public bool PrintYN { get; set; }
        // Add del file, upload file
        // Quan add : 2020/08/18
        public bool DelFileYN { get; set; }
        public bool UploadFileYN { get; set; }
        //bao add
        public bool INVENTORY_YN{ get; set; }
        public bool PURCHASE_ORDER_YN { get; set; }
        public bool EXPORT_EXCEL_ICUBE_YN { get; set; }


    }
}
