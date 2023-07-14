using DocumentFormat.OpenXml.Drawing.Pictures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_CheckInInfo
    {
        public int no { get; set; }
        public string userId { get; set; }
        public string storeName { get; set; }
        public Byte[]? takenPhoto { get; set; }
        public Byte[]? idCardPhoto { get; set; }
        public Byte[]? faceCheckIn { get; set; }
        public DateTime? useDate { get; set; }
        public string userName { get; set; }
        public DateTime? birthday { get; set; }
        public bool gender { get; set; }
        public string genderName { get; set; }
        public bool approveReject { get; set; }
        public string phoneNumber { get; set; }
        public int storeNo { get; set; }
        public string photoTakenBase64 { get; set; }
        public string photoCardBase64 { get; set; }
    }
}
