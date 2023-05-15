using InfrastructureCore.Common;
using System.ComponentModel.DataAnnotations;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ComCodeDtls : HLCoreCrudModel
    {
        public int NO { get; set; } // for display No in view
        public string GROUP_CD { get; set; }
        [Required(ErrorMessage = "Code must be inputed!")]
        public string BASE_CODE { get; set; }
        [Required(ErrorMessage = "Code name must be inputed!")]
        public string BASE_NAME1 { get; set; }
        public string BASE_NAME2 { get; set; }
        public string BASE_NAME3 { get; set; }
        public string BASE_NAME4 { get; set; }
        public string BASE_NAME5 { get; set; }

        public string BASE_NAME { get; set; }
        public int SORT { get; set; }
        public bool USE_YN { get; set; }
    }
}
