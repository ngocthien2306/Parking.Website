using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class MESEmployees
    {
        [DisplayName("NoA")]
        public int No { get; set; }
        public string EmployeeNumber { get; set; }
        public string OrgNumber { get; set; }
        public string EmployeeNameKr { get; set; }
        public string EmployeeNameEng { get; set; }
        public string Level { get; set; }       
        public string Company { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string RfidTag { get; set; }
        public string UseYN { get; set; }
        public string PassWord { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string CompanyType { get; set; }

        public string CountryType { get; set; }
    }
}
