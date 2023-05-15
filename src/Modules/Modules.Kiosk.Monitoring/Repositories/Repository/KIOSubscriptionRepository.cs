using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Kiosk.Monitoring.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Monitoring.Repositories.Repository
{
    public class KIOSubscriptionRepository : IKIOSubscriptionRepository
    {
        private readonly static string SP_SUBSCRIPTION_HISTORY = "SP_SUBSCRIPTION_HISTORY";
        public List<KIO_SubscriptionHistory> GetSubscriptionHistory(string storeNo, string startTime, string endTime)
        {
            List<KIO_SubscriptionHistory> subscriptionHistory = new List<KIO_SubscriptionHistory>();
            try
            {
                using(var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[4];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@StartTime";
                    arrParam[3] = "@EndTime";
                    object[] arrValue = new object[4];
                    arrValue[0] = "GetSubscriptionHistory";
                    arrValue[1] = storeNo;
                    arrValue[2] = startTime;
                    arrValue[3] = endTime;
                    subscriptionHistory = connection.ExecuteQuery<KIO_SubscriptionHistory>(SP_SUBSCRIPTION_HISTORY, arrParam, arrValue).ToList();
                    return subscriptionHistory;
                }
            }
            catch
            {
                return subscriptionHistory;
            } 
        }
    }
}
