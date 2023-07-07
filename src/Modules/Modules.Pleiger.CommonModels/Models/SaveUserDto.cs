using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class SaveUserDto
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public bool gender { get; set; }
        public string phone { get; set; }
        public DateTime? birthday { get; set; }
        public string imgTakenPath { get; set; }
        public string imgCardPath { get; set; }
    }
    public class SaveUserProfile : SaveUserDto
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneCode { get; set; }
        public string email { get; set; }
        public string idCardBackPhotoPath { get; set; }
        public string idCardFrontPhotoPath { get; set; }
        public string idCardPhotoPath { get; set; }
    }
}
