namespace InfrastructureCore.Models.Site
{
    public class SYSiteSettings
    {
        public int SiteSettingID { get; set; }
        public string SiteSettingName { get; set; }
        public bool IsActive { get; set; }

        // Icon Page
        public string IconPath { get; set; }

        // Login Page
        public string LoginBackgroundImage { get; set; }
        public string LoginTitle { get; set; }
        public string LoginTextColor { get; set; }

        // Logo
        public string LogoPath { get; set; }
        public string LogoName { get; set; }
        public string LogoBackgroundColor { get; set; }

        // Menu
        public string MenuType { get; set; }
        public string SideBarType { get; set; }

        // Side Menu
        public string SideParentACtiveBackgroundColor { get; set; }
        public string SideActiveBackgroundColor { get; set; }
        public string SideHoverBackgroundColor { get; set; }

        // Top Menu
        public string TopBackgroundColor { get; set; }
        public string TopActiveBackgroundColor { get; set; }
        public string TopHoverBackgroundColor { get; set; }

        // Footer
        public string FooterVisible { get; set; }
        public string FooterBackgroundColor { get; set; }
        public string FooterLeftText { get; set; }
        public string FooterRightText { get; set; }

        /// <summary>
        /// partial view path 
        /// or 
        /// LeftMenuComponent name
        /// </summary>
        public string LeftMenuComponentUrl { get; set; }
        
    }
}
