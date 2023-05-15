using InfrastructureCore;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Kiosk.Management.Repositories.IRepositories
{
    public interface IKIOAccountRepository
    {
        List<KIO_AccountMgt> GetAccountMgt(string userId);
        List<KIO_UseRegisteredStore> GetUserRegisteredStore(string storeNo, string userId);
       
        Result SaveAccountMgt(string userId, bool status);
        Result DeleteUserOutStore(string userId, string storeNo);
        Result AddUserToStore(string userId, string storeNo);
    }
}
