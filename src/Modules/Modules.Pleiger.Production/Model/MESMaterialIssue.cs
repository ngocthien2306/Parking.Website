using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESMaterialIssue
    {
        public int No { get; set; }
        public string MaterialIssueNo { get; set; }
        public string UseTeam { get; set; }
        public string UseTeamCode { get; set; }
        public DateTime MaterialIssueDate { get; set; }
        public string ITEMCode { get; set; }
        public string ItemName { get; set; }
        public string ProductionProjectCode { get; set; }
        public string Creator { get; set; }
        public string CreatorUserName { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }

        public string SlipNumber { get; set; }
    }
}
