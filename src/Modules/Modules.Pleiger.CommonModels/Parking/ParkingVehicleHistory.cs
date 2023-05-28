using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Parking
{
    public class ParkingVehicleHistory
    {
        public int no { get; set; }
        public int storeNo { get; set; }
        public string location { get; set; }
        public string locationName { get; set; }
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
        public string typeTransport { get; set; }
        public string typePlate { get; set; }
    }
}
