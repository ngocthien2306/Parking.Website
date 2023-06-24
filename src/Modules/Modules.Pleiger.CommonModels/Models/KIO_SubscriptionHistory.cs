using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_SubscriptionHistory
    {
        public int no { get; set; }
        public string userId { get; set; }
        public int storeNo { get; set; }
        public string location { get; set; }
        public string locationName { get; set; }
        public string storeName { get; set; }
        public string userName { get; set; }
        public DateTime? birthday { get; set; }
        public bool gender { get; set; }
        public string phoneNumber { get; set; }
        public DateTime? registDate { get; set; }
        public string approvalType { get; set; }
        public int? lastSimilarity { get; set; }
        public bool approveReject { get; set; }
        public DateTime? useDate { get; set; }
        public string userType { get; set; }
        public string userStatus { get; set; }
        public Byte[]? takenPhoto { get; set; }
        public Byte[]? idCardPhoto { get; set; }
        public Byte[]? idCardBackPhoto { get; set; }
        public Byte[]? idCardFrontPhoto { get; set; }
        public string? takenPhotoBase64 { get; set; }
        public string? idCardPhotoBase64 { get; set; }
        public string? idCardBackPhotoBase64 { get; set; }
        public string? idCardFrontPhotoBase64 { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string identityNo { get; set; }
    }
}
