using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.IRepository
{
    public interface IKIOEquipmentRepository
    {
        Result SaveDataStoreDevice(KIO_StoreDevice storeDevice, string userId);
        Result DeleteStoreDevice(string storeNo);
        Result UpdateStatusDevice(string storeNo, string storeDeviceNo, bool status);
    }
}
