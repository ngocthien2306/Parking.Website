using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_AdMgt
    {
        public int no { get; set; }
        public int? adNo { get; set; }
        public string adType { get; set; }
        public string adTypeName { get; set; }
        public string adName{ get; set; }
        public bool adLocation { get; set; }
        public DateTime? periodStartDate { get; set; }
        public DateTime? periodEndDate { get; set; }
        public string period { get; set; }

        public DateTime? dayStartTime { get; set; }
        public DateTime? dayEndTime { get; set; }
        public string dayTime { get; set; }

        public bool? adStatus { get; set; }
        public DateTime? registDate { get; set; }
        public string resitUser { get; set; }
        public string attachFilePath { get; set; }
        public int? version { get; set; }

    }


    public class AdDto
    {
        public string adNo { get; set; }
        public string adType { get; set; }
    }


    public partial class RequestInfo
    {

        public Guid KeyID { get; set; }
        public object ID { get; set; }
        public int Top { get; set; }
        public string FuncWhere { get; set; }
        public string TSQL { get; set; }
        public object Info { get; set; }
        public Type TypeInfo { get; set; }
        public object TypeCode { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public bool IsBranchID { get; set; }
        public int BranchID { get; set; }
        public object BranchCode { get; set; }
        public string Owner { get; set; }
        public int Option { get; set; }
        //**----------------------------------------------------------------------------------------------------------
        public int PageSize { get; set; } = 100;
        public int PageIndex { get; set; } = 0;
        public string TSQLTotal { get; set; } = "";
        //**----------------------------------------------------------------------------------------------------------
        public string Columns { get; set; } = "[]";
        public dynamic Data { get; set; }
        public dynamic Details { get; set; }
        public object GroupCode { get; set; } = "";
        //**----------------------------------------------------------------------------------------------------------
        public string ModelName { get; set; }
        public int ClientServiceID { get; set; }
        public Guid AppKey { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        //--------------------------------------------

        public RequestInfo()
        {
            KeyID = Guid.NewGuid();
            ID = 0;
            Top = 9999;
            TSQL = "";
            Info = null;
            TypeInfo = null;
            //BranchID = SystemSetting.BranchSelectID;
            Data = null;
            AppKey = Guid.Empty;
            ClientServiceID = 0;
            ModelName = "";
            UserName = "";
            UserName = null;
        }
        public RequestInfo(object id, int top)
        {
            KeyID = Guid.NewGuid();
            ID = id;
            Top = top;
            TSQL = "";
            Info = null;
            TypeInfo = null;
            Data = null;
            AppKey = Guid.Empty;
            ClientServiceID = 0;
            ModelName = "";
            UserName = "";
            UserName = null;
        }


        public DateTime GetFromDate()
        {
            return DateTime.Parse(FromDate + " 00:00:00");
        }
        public DateTime GetToDate()
        {
            return DateTime.Parse(ToDate + " 00:00:00");
        }

    }
    public class tblAdMgtInfo
    {
        private object _id;
        public int? AdNo { get; set; }
        public string AdType { get; set; }
        public string AdName { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public DateTime DayStartTime { get; set; }
        public DateTime DayEndTime { get; set; }
        public bool AdStatus { get; set; }
        public DateTime RegistDate { get; set; }
        public string ResitUser { get; set; }
        public bool AdLocation { get; set; }
        public string AttachFilePath { get; set; }
        public int? Version { get; set; }
        public string LocalPath { get; set; }
        public bool Check { get; set; }

    }
}
