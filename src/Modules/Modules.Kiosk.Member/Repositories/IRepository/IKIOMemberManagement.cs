using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Member.Repositories.IRepository
{
    public interface IKIOMemberManagement
    {
        List<KIO_SubscriptionHistory> GetMemberManagement(string storeNo, string userId, int lessMonth, int onceRecently);
        List<KIO_SubscriptionHistory> GetMemberManagementDetail(string storeNo, string userId);

        List<KIO_UserHistory> GetUserHistory(string userId);

        Result SaveDataMember(SaveUserDto saveUserDto);
        Result DeleteDataMember(string storeNo, string userId);

    }
}
