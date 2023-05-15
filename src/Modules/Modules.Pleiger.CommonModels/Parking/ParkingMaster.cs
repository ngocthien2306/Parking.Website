using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Parking
{
    public class ParkingMaster
    {
        public int No { get; set; }
        public int SideId { get; set; }
        public string Coordinates { get; set; }
        public string NameParking { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Capacity { get; set; }
        public int RemainSize { get; set; }
        public bool Status { get; set; }
        public string Location { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class ParkingMasterRequest {
        
    }
}
