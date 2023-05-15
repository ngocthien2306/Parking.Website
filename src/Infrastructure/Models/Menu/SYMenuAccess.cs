using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Models.Menu
{
    /// <summary>
    /// MenuPermissions
    /// </summary>
    public class SYMenuAccess
    {
        public int MENU_ID { get; set; }
        public bool SEARCH_YN { get; set; }
        public bool CREATE_YN { get; set; }
        public bool SAVE_YN { get; set; }
        public bool DELETE_YN { get; set; }
        public bool EDIT_YN { get; set; }
        public bool EXCEL_YN { get; set; }// export
        public bool PRINT_YN { get; set; }
        // Quan add del, Upload File 2020/08/18       
        public bool DELETE_FILE_YN { get; set; }
        public bool UPLOAD_FILE_YN { get; set; }
        //bao add import excel 19/08/2020
        public bool IMPORT_EXCEL_YN { get; set; }// Import excel
        public SYMenuAccess()
        {
            SEARCH_YN = false;
            CREATE_YN = false;
            SAVE_YN = false;
            EDIT_YN = false;
            DELETE_YN = false;
            EXCEL_YN = false;
            PRINT_YN = false;
            // Quan add del, Upload File 2020/08/18      
            DELETE_FILE_YN = false;
            UPLOAD_FILE_YN = false;
            //bao add import excel 19/08/2020
            IMPORT_EXCEL_YN = false;
        }
    }
   
}
