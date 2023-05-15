using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Models.Common
{
    /// <summary>
    /// MenuPermissions
    /// </summary>
    public class ToolbarView
    {
        public int MENU_ID { get; set; }
        public bool SEARCH_YN { get; set; }
        public bool CREATE_YN { get; set; }
        public bool SAVE_YN { get; set; }
        public bool DELETE_YN { get; set; }
        public bool EDIT_YN { get; set; }
        public bool EXCEL_YN { get; set; }
        public bool PRINT_YN { get; set; }
    }
}
