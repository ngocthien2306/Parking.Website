namespace InfrastructureCore.Models.Menu
{
    public class SYGroupAccessMenus
    {
        public int GROUP_ID { get; set; }
        public int MENU_ID { get; set; }
        public bool SEARCH_YN { get; set; }
        public bool CREATE_YN { get; set; }
        public bool SAVE_YN { get; set; }
        public bool DELETE_YN { get; set; }
        public bool EDIT_YN { get; set; }
        public bool EXCEL_YN { get; set; }
        public bool PRINT_YN { get; set; }
        public int SITE_ID { get; set; }

        public string GROUP_NAME { get; set; }
        public string DESCRIPTION { get; set; }

        // Add del file, upload file
        // Quan add : 2020/08/18
        public bool DELETE_FILE_YN { get; set; }
        public bool UPLOAD_FILE_YN { get; set; }
        public bool INVENTORY_YN { get; set; }
        public bool PURCHASE_ORDER_YN { get; set; }
        public bool EXPORT_EXCEL_ICUBE_YN { get; set; }
        public int DELETE_FILE_SUM { get; set; }
        public int UPLOAD_FILE_SUM { get; set; }   
        public int SEARCH_YN_SUM { get; set; }
        public int CREATE_YN_SUM { get; set; }
        public int SAVE_YN_SUM { get; set; }
        public int DELETE_YN_SUM { get; set; }
        public int EDIT_YN_SUM { get; set; }
        public int EXCEL_YN_SUM { get; set; }
        public int PRINT_YN_SUM { get; set; }
        public int INVENTORY_YN_SUM { get; set; }
        public int PURCHASE_ORDER_YN_SUM { get; set; }
        public int EXPORT_EXCEL_ICUBE_SUM { get; set; }

    }
}
