using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Parking
{
    public class ParkingCheckin
    {
        public int trackNumber { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public double fee { get; set; }
        public int storeNo { get; set; }
        public string storeName { get; set; }
        public int capacity { get; set; }
        public double locationX { get; set; }
        public double locationY { get; set; }
        public int userId { get; set; }
        public string detectInFace { get; set; }
        public string detectOutFace { get; set; }
        public string plateIn { get; set; }
        public string plateOut { get; set; }
        public int vehicleId { get; set; }
        public string plateNum { get; set; }
        public string typeTransport { get; set; }
        public string typePlate { get; set; }
        public string vehicleStatus { get; set; }
    }

    public class tblTrack
    {
        public int Id { get; set; }
        public string trackNumber { get; set; }
        public int vehicleId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public float fee { get; set; }
        public int siteId { get; set; }
        public float locationX { get; set; }
        public float locationY { get; set; }
        public int userId { get; set; }
        public string detectInFace { get; set; }
        public string detectOutFace { get; set; }
        public string plateIn { get; set; }
        public string plateOut { get; set; }
    }
}
