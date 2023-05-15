using InfrastructureCore;
using Modules.Admin.Models;
using System.Collections.Generic;

namespace Modules.Admin.Services.IService
{
    public interface IWebAuthentication
    {
        Result CheckLogIn(string username, string password);
        //TL
        Result CheckLogInTL(string siteCode, string username, string password);

    }
}
