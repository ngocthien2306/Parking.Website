namespace Modules.Common.Models.Web
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuPath { get; set; }
        public int MenuLevel { get; set; }
        public int MenuParentID { get; set; }
        public int MenuSeq { get; set; }
        public string AdminLevel { get; set; }
        public string MenuType { get; set; }
        public int ProgramID { get; set; }
        public string MenuDesc { get; set; }
        public string MobileUse { get; set; }
        public string IntraUse { get; set; }
        public string StartupPageUse { get; set; }
        public string IsCanClose { get; set; }
    }
}
