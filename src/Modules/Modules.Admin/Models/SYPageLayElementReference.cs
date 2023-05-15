using System.ComponentModel.DataAnnotations;

namespace Modules.Admin.Models
{
    public class SYPageLayElementReference
    {
        public int ID { get; set; }
        public int PAG_ID { get; set; }
        public string PEL_ID { get; set; }
        public string PEL_ID_TO { get; set; }
        [Required]
        public string REF_TYPE { get; set; }
        [Required]
        public string SOURCE_COL_NM { get; set; }
        [Required]
        public string TARGET_COL_NM { get; set; }
        public string IO_FL { get; set; }
        public string DATA_MAP_ADDON { get; set; }
    }
}
