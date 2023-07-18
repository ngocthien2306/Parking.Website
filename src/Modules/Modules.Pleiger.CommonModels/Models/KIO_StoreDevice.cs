using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_StoreDevice
    {
        public int no { get; set; }
        public int storeNo { get; set; }
        public int storeDeviceNo { get; set; }
        public string deviceTypeName { get; set; }
        public DateTime? registDate { get; set; }
        public string deviceType { get; set; }
        public string deviceName { get; set; }
        public string deviceKeyNo { get; set; }
        public string devicePublicIp { get; set; }
        public int deviceUsePort { get; set; }
        public string registUserId { get; set; }
        public bool deviceStatus { get; set; }
        public string rdpPath { get; set; }
        public string deviceKey { get; set; }
        public string network { get; set; }
        public string networkName { get; set; }
        public string threshold { get; set; }
        public string thresholdName { get; set; }
        public int? faceLimit { get; set; }
        public int? minFaceSize { get; set; }
        public int? attemp { get; set; }

    }

    public class StoreDeviceDto
    {
        public int storeNo { get; set; }
        public int storeDeviceNo { get; set; }
        public bool deviceStatus { get; set; }
    }
}
