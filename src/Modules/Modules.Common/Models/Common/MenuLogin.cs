namespace Modules.Common.Models.Common
{
    public class MenuLogin
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
        public string StartUpPage { get; set; }
        public string IsCanClose { get; set; }
        public string IsMobileUse { get; set; }
    }
}
