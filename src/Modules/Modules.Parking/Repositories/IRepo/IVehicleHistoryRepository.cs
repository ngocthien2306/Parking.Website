using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using Modules.Pleiger.CommonModels.Parking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Parking.Repositories.IRepo
{
     
    public interface IVehicleHistoryRepository
    {
        List<ParkingVehicleHistory> GetMemberManagement(string storeNo, string userId, int lessMonth, int onceRecently);
        List<ParkingHistoryDetail> GetMemberManagementDetail(string storeNo, string userId, string lp);
        List<VehiceInfo> GetVehiceInfo(string storeNo, string userId, string lp, string vehicleId);
        List<KIO_UserHistory> GetUserHistory(string userId);
        Result SaveDataMember(SaveUserDto saveUserDto);
        Result DeleteDataMember(string storeNo, string userId);
    }
}
