using System.ComponentModel.DataAnnotations;

namespace InfrastructureCore.Models.Module
{
    public class SYModulesMg
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public string NAME { get; set; }
        public bool IS_BUNDLE_WITH_HOST { get; set; }
        [Required]
        public string VERSION { get; set; }
        [Required]
        public bool IS_ACTIVE { get; set; }
        public bool USED_YN { get; set; }
        public int SITE_ID { get; set; }
    }
}
