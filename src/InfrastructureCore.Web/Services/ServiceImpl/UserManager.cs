using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Data;
using InfrastructureCore.Web.Services.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using InfrastructureCore.Extensions;
using InfrastructureCore.DAL;
using System.Data;

namespace InfrastructureCore.Web.Services.ServiceImpl
{
    public class UserManager : IUserManager
    {
        private ApplicationDbContext storage;
        private const string SP_WEB_SY_USER = "SP_Web_SY_User";

        public UserManager(ApplicationDbContext storage)
        {
            this.storage = storage;
        }

        public ChangePasswordResult ChangePassword(string identifier, string password)
        {
            SYUser credential = this.storage.Users.FirstOrDefault(c => c.UserName == identifier);

            if (credential == null)
                return new ChangePasswordResult(success: false, error: ChangePasswordResultError.CredentialNotFound);


            credential.Password = PasswordExtensions.HashPassword(password);
            credential.LastPassChange = DateTime.Now;
            this.storage.Users.Update(credential);
            this.storage.SaveChanges();
            return new ChangePasswordResult(success: true);

        }

        public SYUser GetCurrentUser(HttpContext httpContext)
        {
            string currentUserId = this.GetCurrentUserId(httpContext);

            if (string.IsNullOrEmpty(currentUserId))
                return null;

            return this.storage.Users.Find(currentUserId);
        }

        public string GetCurrentUserId(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return string.Empty;

            Claim claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
                return string.Empty;

            return claim.Value;
        }

        public SYUser GetUserDataByUserCode(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@UserCode" },
                    new object[] { "GetListDataByUserCode", UserCode });
                return result.FirstOrDefault();
            }
        }

        public async Task SignIn(HttpContext httpContext, SYUser user, bool isPersistent = false)
        {
            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = isPersistent }
            ).ConfigureAwait(true);
        }

        public async Task SignOut(HttpContext httpContext)
        {
            httpContext.Session.Clear();
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(true);
        }

        public ValidateResult Validate(string siteCode, string identifier, string password)
       {
            var site = this.storage.Sites.FirstOrDefault(c => c.SiteCode == siteCode);
            if (site == null)
            {
                return new ValidateResult(success: false, error: ValidateResultError.SiteNotFound);
            }

            var credential = this.storage.Users.FirstOrDefault(c => c.UserName == identifier && c.SiteID == site.SiteID);

            if (credential == null)
                return new ValidateResult(success: false, error: ValidateResultError.CredentialNotFound);

            if (!string.IsNullOrEmpty(password))
            {
                //var hashedPassword = PasswordExtensions.HashPassword(password);
                // var res = PasswordExtensions.VerifyPassword(hashedPassword,credential.Password);
                var res = PasswordExtensions.VerifyPassword(credential.Password, password);

                if (!res)
                {
                    
                    credential.IsCount = (credential.IsCount ?? 0) + 1;
                    int passMaxFailedCnt = site.MaxLogFail.Value;
                    //int passMaxFailedCnt = 5;
                    if (credential.IsCount > passMaxFailedCnt)
                        credential.IsBlock = true;
                    storage.Update(credential);
                    storage.SaveChanges();
                    if(credential.IsBlock==true)
                    {
                        return new ValidateResult(success: false, error: ValidateResultError.IsBlocked, user: credential);

                    }
                    else
                    {
                        return new ValidateResult(success: false, error: ValidateResultError.PasswordNotValid, user: credential);
                    }
                }                
            }

            // Validate more
            //PasswordMostExpired,
            //PasswordExpired,
            //IsBlocked
            var timelog = DateTime.Now.Subtract(credential.LastLoggedIn??DateTime.Now);
            if (true.Equals(credential.IsBlock) && timelog.TotalMinutes<=5)
            {                   
                return new ValidateResult(success: false, error: ValidateResultError.IsBlocked);              
            }

            int daysToChange = site.ChangePassPeriod.Value; // get from SiteSettings
            //int daysToChange = 90; // get from SiteSettings

            if (credential.LastPassChange == null || credential.LastPassChange.Value.AddDays(daysToChange) <= DateTime.Today)
            {
                return new ValidateResult(success: false, error: ValidateResultError.PasswordExpired);
            }

            var passChangeDate = credential.LastPassChange.Value.AddDays(daysToChange);
            var ts = passChangeDate.Subtract(DateTime.Today);
            if (ts.Days <= 10)
            {
                return new ValidateResult(user: this.storage.Users.Find(credential.UserID), success: true, error: ValidateResultError.PasswordMostExpired);
            }         
            credential.IsCount = 0;
            credential.IsBlock = false;
            credential.LastLoggedIn = DateTime.Now;
            storage.Update(credential);
            storage.SaveChanges();

            return new ValidateResult(user: this.storage.Users.Find(credential.UserID), success: true, error: ValidateResultError.NoError);
        }

        #region Privates

        private IEnumerable<Claim> GetUserClaims(SYUser user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            claims.Add(new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName));

            claims.Add(new Claim(ClaimTypes.Expired, TimeSpan.FromDays(1).ToString(), ClaimValueTypes.DaytimeDuration)); // TODO

            claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetUserRoleClaims(SYUser user)
        {
            List<Claim> claims = new List<Claim>();
            IEnumerable<int> roleIds = this.storage.UsersInGroup.Where(ur => ur.USER_CODE == user.UserID).Select(ur => ur.GROUP_ID).ToList();

            //if (roleIds != null)
            //{
            //    foreach (int roleId in roleIds)
            //    {
            //        if(this.storage.UserGroups != null)
            //        {
            //            SYUserGroups role = this.storage.UserGroups.Find(roleId);

            //            claims.Add(new Claim(ClaimTypes.Role, role.GROUP_NAME));
            //        }

            //    }
            //}

            return claims;
        }

        #endregion


        // Quan add 2021-01-18
        public List<SYAuditLogTracking> GetAuditLogTracking(string UserName, int? SiteID,string StarDate, string EndDate)
        {
            var result = new List<SYAuditLogTracking>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[5];
                arrParams[0] = "@Method";
                arrParams[1] = "@UserName";
                arrParams[2] = "@SITE_ID";
                arrParams[3] = "@StartDate";
                arrParams[4] = "@EndDate";
                object[] arrParamsValue = new object[5];
                arrParamsValue[0] = "GetAuditLogTracking";
                arrParamsValue[1] = UserName;
                arrParamsValue[2] = SiteID;
                arrParamsValue[3] = StarDate;
                arrParamsValue[4] = EndDate;
                var data = conn.ExecuteQuery<SYAuditLogTracking>("AuditLogTracking", arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        public Result SaveAuditLogTracking(SYAuditLogTracking model)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var resultIns = -1;
                    try
                    {
                        resultIns = conn.ExecuteNonQuery("AuditLogTracking", CommandType.StoredProcedure,
                            new string[] {  
                                "@Method", 
                                "@USER_INFO",
                                "@ACTION_TYPE",
                                "@SOURCE_IP", 
                                "@URL",
                                "@DATE_LOG",
                                "@MESSAGE",
                                "@HEADER_MAP",
                                "@USERNAME",
                                "@PASSWORD",
                                "@SITE_ID"

                            },
                            new object[] { 
                                "InsertAuditLogTracking",
                                model.USER_INFO,
                                model.ACTION_TYPE,
                                model.SOURCE_IP,
                                model.URL,
                                model.DATE_LOG,
                                model.MESSAGE,
                                model.HEADER_MAP,
                                model.USERNAME,
                                model.PASSWORD,
                                model.SITE_ID
                            },
                            transaction);
                        transaction.Commit();
                        if (resultIns > 0)
                        {
                            return new Result
                            {
                                Success = true,
                                //Message = MessageCode.MD0004
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                //Message = MessageCode.MD0005
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Save data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }

            return result;
        }
    }
}
