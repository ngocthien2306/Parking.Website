using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.IRepository
{
    public interface IKIOPreferenceRepository
    {
        Result SaveEnvSettings(KIO_StoreEnvSettings envSettings);
        Result SaveAlarmStoreMessage(KIO_AlarmToStoreMessage alarmToStoreMessage);

        List<KIO_StoreEnvSettings> GetStoreEnvSettings(string storeNo, string envNo);
        List<KIO_AlarmToStoreMessage> GetAlarmToStoreMessage(string storeNo, string alarmNo);
    }
}
