

using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System.Collections.Generic;

namespace Modules.Kiosk.Monitoring.Repositories.IRepository
{
    public interface IKIOCheckInRepository
    {
        public List<KIO_UserStore> GetUserStoreMgt(string storeNo, string userId, string userType);
        public Result UpdateApproveRejectUser(string userId, bool status);
        public Result UpdateApproveRejectRemoveUser(string userId, bool status);

        public List<KIO_CheckInInfo> GetCheckInInfo(string storeNo, string startDate, string endDate, int byMin);
        public List<KIO_CheckInInfo> GetCheckInInfo(string storeNo, string startDate, string endDate, int byMin, string userId);
        public KIO_CheckInInfo GetPhotoById(string userId);
    }
}
