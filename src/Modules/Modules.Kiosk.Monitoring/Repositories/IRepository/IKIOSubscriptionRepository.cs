using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Monitoring.Repositories.IRepository
{
    public interface IKIOSubscriptionRepository
    {
        List<KIO_SubscriptionHistory> GetSubscriptionHistory(string storeNo, string startTime, string endTime);

    }
}
