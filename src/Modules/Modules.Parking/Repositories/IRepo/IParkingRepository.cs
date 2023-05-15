
using InfrastructureCore;
using Modules.Pleiger.CommonModels.Parking;
using System.Collections.Generic;

namespace Modules.Parking.Repositories.IRepo
{
    public interface IParkingRepository
    {
        List<ParkingMaster> GetListParkingMaster(string id);
        Result SaveParkingMaster(ParkingMaster parkingMaster);
        Result DeletParking(int sideId);
    }
}
