using InfrastructureCore.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Modules.Pleiger.Models
{
    public class MES_ComCodeMst : HLCoreCrudModel
    {
        public int NO { get; set; } // for display No in view
        [Required(ErrorMessage = "Group code must be inputed!")]
        public string GROUP_CD { get; set; }
        [Required(ErrorMessage = "Group name must be inputed!")]
        public string GROUP_NM1 { get; set; }
        public string GROUP_NM2 { get; set; }
        public string GROUP_NM3 { get; set; }
        public string GROUP_NM4 { get; set; }
        public string GROUP_NM5 { get; set; }
        public string DESCRIPTION { get; set; }
        public bool USE_YN { get; set; }
        public string REMARK { get; set; }

        //public string __original { get; set; }
        //public string __inserted { get; set; }
        //public string __updated { get; set; }
        //public string __removed { get; set; }

        //[JsonProperty("__deleted__")]
        //protected bool __deleted__ { get; set; }

        //[JsonProperty("__created__")]
        //protected bool __created__ { get; set; }

        //[JsonProperty("__modified__")]
        //protected bool __modified__ { get; set; }
    }
}
