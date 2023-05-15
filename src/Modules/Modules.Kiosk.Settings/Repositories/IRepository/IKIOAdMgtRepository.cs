using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.IRepository
{
    public interface IKIOAdMgtRepository
    {
        List<KIO_AdMgt> GetAdMgt(KIO_AdMgt adMgt);
        List<KIO_StoreMaster> GetAdMgtStore(string adNo);
        Result SaveAdMgt(KIO_AdMgt adMgt, string listStoreNo);
        Result DeleteAdMgt(string adNo, bool checkDeleteAd);
    }
}
