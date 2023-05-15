namespace InfrastructureCore.Models.Menu
{
    public class SYMenu
    {
        public int No { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuNameEng { get; set; }
        public string MenuPath { get; set; }
        public int MenuLevel { get; set; }
        public int MenuParentID { get; set; }
        public int MenuSeq { get; set; }
        public int SiteID { get; set; }
        public string AdminLevel { get; set; }
        public string MenuType { get; set; }
        public int ProgramID { get; set; }
        public string MenuDesc { get; set; }
        public string MobileUse { get; set; }
        public string IntraUse { get; set; }
        public string StartupPageUse { get; set; }
        public string IsCanClose { get; set; }
        public int MenuIcon { get; set; }
        public string MenuIconCode { get; set; }
        public string UseYN { get; set; }
        public string HiddenYN { get; set; }
        public bool Selected { get; set; }
        public int MenuIDActivefirst { get; set; }
        public string MenuPathActivefirst { get; set; }
        public string MenuNameActivefirst { get; set; }
        public string GroupName { get; set; }       

    }
}
