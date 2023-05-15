using Modules.Admin.Services.IService;
using InfrastructureCore;
using System;
using System.Collections.Generic;
using InfrastructureCore.DAL;
using System.Linq;
using System.ServiceProcess;
using Serilog;
using Modules.Common.Models;
using InfrastructureCore.Models.Module;

namespace Modules.Admin.Services.ServiceImp
{
    public class ModulesService : IModulesService
    {
        private static readonly string SP_WEB_SY_MODULES_MANAGEMENT = "SP_Web_SYModulesMg";

        #region Modules Management By Site

        #endregion

        public List<SYModulesMg> GetAllModulesBySite()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYModulesMg>(SP_WEB_SY_MODULES_MANAGEMENT,
                    new string[] { "@DIV" },
                    new object[] { CommonAction.SELECT });
                return result.ToList();
            }
        }

        public Result SavedDataModulesBySite(SYModulesMg item, string action)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                {
                    actionType = CommonAction.INSERT;
                }
                else
                {
                    actionType = CommonAction.UPDATE;
                }
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_WEB_SY_MODULES_MANAGEMENT,
                        new string[] { "@DIV", "@ID", "@NAME", "@IS_BUNDLE_WITH_HOST", "@VERSION", "@IS_ACTIVE", "@SITE_ID" },
                        new object[] { actionType, item.ID, item.NAME, item.IS_BUNDLE_WITH_HOST, item.VERSION, item.IS_ACTIVE, item.SITE_ID });
                    if (result == -1)
                    {
                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Save changed data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save changed data not success! + Exception: " + ex.ToString(),
                    };
                }

            }
        }

        public Result DeletedDataModulesBySite(SYModulesMg item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_WEB_SY_MODULES_MANAGEMENT,
                        new string[] { "@DIV", "@ID"},
                        new object[] { CommonAction.DELETE, item.ID });
                    if (result == -1)
                    {
                        return new Result
                        {

                            Success = true,
                            Message = "Delete success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Delete not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Delete not success! + Exception: " + ex.ToString(),
                    };
                }

            }
        }

        public Result ApplyModulesConfig()
        {
            try
            {
                using (ServiceController controller = new ServiceController())
                {
                    string appPoolName = "localhost:57965";
                    controller.MachineName = @"IIS://" + System.Environment.MachineName + "/W3SVC/AppPools/" + appPoolName;
                    controller.ServiceName = "W3SVC"; // i.e “w3svc”

                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        // Start the service
                        controller.Start();

                        Log.Debug("IIS has been started successfully, now checking again for webservice availability") ;

                    }
                    else
                    {
                        // Stop the service
                        controller.Stop();

                        // Start the service
                        controller.Start();

                        Log.Debug("IIS has been restarted successfully");

                    }

                }

                return new Result
                {
                    Success = true,
                    Message = "Save changed data success!"
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not success! + Exception: " + ex.ToString(),
                };
            }
        }
    }
}
