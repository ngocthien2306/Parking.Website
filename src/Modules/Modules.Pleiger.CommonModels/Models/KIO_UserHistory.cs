using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_UserHistory
    {
        public int no { get; set; }
        public int userLoginNo { get; set; }
        public string userId { get; set; }
        public string approvalType { get; set; }
        public int? lastSimilarityRate { get; set; }
        public bool approveReject { get; set; }
        public DateTime? loginTime { get; set; }
        public string loginIp { get; set; }
    }
    public class KIO_UserIdDto
    {
        public string userId { get; set; }
        public string userName { get; set; }
    }
    public class tblUserHistory
    {
        public int UserLoginNo { get; set; }
        public string UserId { get; set; }
        public string ApprovalType { get; set; }
        public int? LastSimilarityRate { get; set; }
        public bool ApproveReject { get; set; }
        public DateTime? LoginTime { get; set; }
        public string LoginIp { get; set; }
    }
    public class tblUserInfo
    {
        public string UserID { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public bool ApproveReject { get; set; }
        public bool UseYN { get; set; }
        public string UserStatus { get; set; }

        public DateTime RegistDate { get; set; }

        public string Desc { get; set; }

        public bool isRemoveTempUser { get; set; }
        public List<string> ListUserId { get; set; }

        public tblUserMgtStoreInfo TblUserMgtStoreInfo { get; set; }
        public tblUserInfo TblUserInfo { get; set; }
        public tblUserPhotoInfo TblUserPhotoInfo { get; set; }
        //public tblStoreUseHistoryInfo TblStoreUseHistoryInfo { get; set; }

    }
    public class tblUserMgtStoreInfo
    {
        public string UserID { get; set; }
        public int StoreNo { get; set; }
        public string Memo { get; set; }
        public DateTime RegistDate { get; set; }
    }
    public class tblUserPhotoInfo
    {
        //-------------------------------------------------------------

        private object _id;

        public int UserPhotoNo { get; set; }
        public string UserID { get; set; }
        public byte[] TakenPhoto { get; set; }
        public byte[] IdCardPhoto { get; set; }
    }

    public class Datas
    {
        public object Data { get; set; }
    }
    public class DataRequest
    {
        public UInt32 Signature { get; set; }
        public int FrameID { get; set; }
        public UInt16 FunctionCode { get; set; }
        public UInt32 DataLength { get; set; }
        public object Data { get; set; }
    }

}
