using System.ComponentModel.DataAnnotations.Schema;

namespace InfrastructureCore.Models.Site
{
    public class SYSite
    {
        // Site Info
        public int SiteID { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }

        // Icon Page
        [NotMapped]
        public string IconPath { get; set; }

        // Login Page
        [NotMapped]
        public string LoginBackgroundImage { get; set; }
        [NotMapped]
        public string LoginTitle { get; set; }
        [NotMapped]
        public string LoginTextColor { get; set; }

        // Logo
        [NotMapped]
        public string LogoPath { get; set; }
        [NotMapped]
        public string LogoName { get; set; }
        [NotMapped]
        public string LogoBackgroundColor { get; set; }
        [NotMapped]
        public string LogoComponentName { get; set; }
        [NotMapped]
        public string AccountComponentName { get; set; }

        // Menu
        [NotMapped]
        public string MenuType { get; set; }
        [NotMapped]
        public string SideBarType { get; set; }

        // Side Menu
        [NotMapped]
        public string SideParentActiveBackgroundColor { get; set; }
        [NotMapped]
        public string SideActiveBackgroundColor { get; set; }
        [NotMapped]
        public string SideHoverBackgroundColor { get; set; }
        [NotMapped]
        public string SideMenuComponentName { get; set; }
        [NotMapped]
        public string ShowLeftMenuBottom { get; set; }
        [NotMapped]
        public string SideMenuBottomComponentName { get; set; }

        // Top Background
        [NotMapped]
        public string TopBackgroundColor { get; set; }
        [NotMapped]
        public string TopBackgroundHoverColor { get; set; }
        [NotMapped]
        public string TopBackgroundActiveColor { get; set; }
        [NotMapped]
        public string TopBackgroundActiveHoverColor { get; set; }

        // Top Text
        [NotMapped]
        public string TopTextColor { get; set; }
        [NotMapped]
        public string TopTextHoverColor { get; set; }
        [NotMapped]
        public string TopTextActiveColor { get; set; }
        [NotMapped]
        public string TopTextActiveHoverColor { get; set; }

        // Footer
        [NotMapped]
        public string FooterVisible { get; set; }
        [NotMapped]
        public string FooterComponentName { get; set; }
        [NotMapped]
        public string FooterBackgroundColor { get; set; }
        [NotMapped]
        public string FooterLeftText { get; set; }
        [NotMapped]
        public string FooterRightText { get; set; }

        // Account Policy
        public int? ChangePassPeriod { get; set; }
        public int? FailedWaitTime { get; set; }
        public int? MaxLogFail { get; set; }
      
        public int? SessionTimeOut { get; set; }
    }
}
