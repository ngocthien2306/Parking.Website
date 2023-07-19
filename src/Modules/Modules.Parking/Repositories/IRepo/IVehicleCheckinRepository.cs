using InfrastructureCore;
using Modules.Pleiger.CommonModels.Parking;
using System.Collections.Generic;

namespace Modules.Parking.Repositories.IRepo
{
    public interface IVehicleCheckinRepository
    {
        public Result UpdateApproveRejectVehicle(string userId, bool status, int no);

        List<ParkingCheckin> GetListVehicleCheckin(string startTime, string endTime, string byMin, string storeNo, string status);
    }
}
