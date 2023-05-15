using InfrastructureCore.Common;
using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_Partner : HLCoreCrudModel
    {
        public int NO { get; set; } // for display No in view
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string PartnerType { get; set; }
        public string PartnerTypeName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string RegistNumber { get; set; }
        public string Email { get; set; }
        public string CountryType { get; set; }
        public string Ceo { get; set; }
        public string Status { get; set; }
        public string FileID { get; set; }
        public string SystemUserType { get; set; }
        public DateTime Created_At { get; set; }



    }
}
