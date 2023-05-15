using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace InfrastructureCore.Web.Services.IService
{
    public interface IUserManager
    {
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="credentialType"></param>
        /// <param name="identifier"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ChangePasswordResult ChangePassword(string identifier, string password);
        ValidateResult Validate(string siteCode, string identifier, string password);
        Task SignIn(HttpContext httpContext, SYUser user, bool isPersistent = false);
        Task SignOut(HttpContext httpContext);

        string GetCurrentUserId(HttpContext httpContext);
        SYUser GetCurrentUser(HttpContext httpContext);
        SYUser GetUserDataByUserCode(string UserCode);
        // Quan add 2021-01-18
        List<SYAuditLogTracking> GetAuditLogTracking(string userName, int? siteID,string starDate,string endDate);
        Result SaveAuditLogTracking(SYAuditLogTracking model);

    }

}
