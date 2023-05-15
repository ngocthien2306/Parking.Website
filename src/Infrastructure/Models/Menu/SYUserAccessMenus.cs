using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Models.Menu
{
    public class SYUserAccessMenus
    {
        public int No { get; set; }
        public int SITE_ID { get; set; }
        public string USER_ID { get; set; }
        public int MENU_ID { get; set; }
        public int MenuId { get; set; }
        public bool SEARCH_YN { get; set; }
        public bool CREATE_YN { get; set; }
        public bool SAVE_YN { get; set; }
        public bool DELETE_YN { get; set; }
        public bool EDIT_YN { get; set; }
        public bool EXCEL_YN { get; set; }
        public bool PRINT_YN { get; set; }
        public string USER_CODE { get; set; }
        // Quan add del, Upload file set permission
        // 2020/08/18
        public bool DELETE_FILE_YN { get; set; }
        public bool UPLOAD_FILE_YN { get; set; }        
        public bool INVENTORY_YN { get; set; }
        public bool PURCHASE_ORDER_YN { get; set; }
        public bool EXPORT_EXCEL_ICUBE_YN { get; set; }
        
        /// <summary>
        /// Login ID
        /// </summary>
        public string USER_NAME { get; set; }
        //add test
        public bool IMPORT_EXCEL_YN { get; set; }
        public string STATE { get; set; }
}
}
