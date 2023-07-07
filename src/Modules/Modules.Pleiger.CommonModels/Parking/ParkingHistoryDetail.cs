using System;
namespace Modules.Pleiger.CommonModels.Parking
{
    public class ParkingHistoryDetail
    {
        public int no { get; set; }
        public int storeNo { get; set; }
        public string location { get; set; }
        public string storeName { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public DateTime? birthday { get; set; }
        public bool? gender { get; set; }
        public string phoneNumber { get; set; }
        public string userType { get; set; }
        public string userStatus { get; set; }
        public DateTime? registDate { get; set; }
        public string plateNumber { get; set; }
        public string detectInFace { get; set; }
        public string plateIn { get; set; }
        public string plateOut { get; set; }
        public string detectOutFace { get; set; }
        public DateTime? endTime { get; set; }


    }
}
