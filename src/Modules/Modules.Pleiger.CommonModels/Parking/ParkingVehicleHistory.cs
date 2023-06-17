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
    public class VehiceInfo
    {
        public int id { get; set; }
        public int plateNum { get; set; }
        public int typeTransport { get; set; }
        public int typePlate { get; set; }
        public int status { get; set; }
        public int userId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public byte[] vehiclePhoto { get; set; }
        public byte[] licensePhoto { get; set; }
        public string vehiclePhotoPath { get; set; }
        public string licensePhotoPath { get; set; }

    }
}
