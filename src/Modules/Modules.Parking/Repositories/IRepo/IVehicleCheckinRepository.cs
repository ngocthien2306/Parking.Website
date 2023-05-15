using Modules.Pleiger.CommonModels.Parking;
using System.Collections.Generic;

namespace Modules.Parking.Repositories.IRepo
{
    public interface IVehicleCheckinRepository
    {
        List<ParkingCheckin> GetListVehicleCheckin(string startTime, string endTime, string byMin, string storeNo);
    }
}
