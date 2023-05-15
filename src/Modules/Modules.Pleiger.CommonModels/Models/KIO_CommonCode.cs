using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_CommonCode
    {
        public int no { get; set; }
        [Required(ErrorMessage = "Common code must be inputed!"), MaxLength(6)]
        public string commonSubCode { get; set; }
        public string commonCode { get; set; }
        [Required(ErrorMessage = "Common name must be inputed!"), MaxLength(20)]
        public string commonSubName1 { get; set; }
        public string commonSubName2 { get; set; }
        public string description { get; set; }
        public bool systemCode { get; set; }
    }
    public class KIO_MasterCode
    {
        public int no { get; set; }
        [Required(ErrorMessage = "Common code must be inputed!"), MaxLength(6)]
        [Key]
        public string commonCode { get; set; }
        [Required(ErrorMessage = "Common name must be inputed!"), MaxLength(20)]
        public string commonName1 { get; set; }
        [MaxLength(20)]
        public string commonName2 { get; set; }
        [MaxLength(20)]
        public string description { get; set; }
        [Required(ErrorMessage = "Please check systemCode")]
        public bool systemCode { get; set; }
    }
}
