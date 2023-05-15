using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Member.Repositories.IRepository
{
    public interface IKIOMemberRemoveMgt
    {
        List<KIO_SubscriptionHistory> GetMemberRemove(string storeNo, string userId, int lessMonth, int onceRecently);
        List<KIO_SubscriptionHistory> GetMemberRemoveDetail(string storeNo, string userId);

        List<KIO_UserHistory> GetRemoveHistory(string userId);

        Result SaveMemberRemove(SaveUserDto saveUserDto);
        Result DeleteMemberRemove(string storeNo, string userId);
    }
}
