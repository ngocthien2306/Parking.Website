using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.IRepository
{
    public interface IKIOStoreRepository
    {
        List<KIO_StoreMaster> GetStoreMastersByUser(string userId);
        List<KIO_StoreMaster> GetStoreMasters(string storeNo, string location, string storeName, string siteType);
        List<KIO_StoreDevice> GetStoreDevices(string storeNo, string deviceName, string type, string key, string ip, string storeDeviceNo);
        List<KIO_UserStore> GetUserStoreMgt(string storeNo, string userId, string userType);
        List<KIO_HistoryUserStore> GetHistoryManager(string storeNo, string userId);
        KIO_UserStore GetUserById(string userId);
        Result SaveStoreMgt(KIO_UserStore userStore, int siteId, string uid, string pass);
        Result ChangesPassword(string password, string userId);
        Result SaveStoreMaster(KIO_StoreMaster storeMaster);


    }
}
