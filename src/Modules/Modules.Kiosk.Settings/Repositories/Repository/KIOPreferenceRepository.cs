using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Modules.Common.Models;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.Repository
{
    public class KIOPreferenceRepository : IKIOPreferenceRepository
    {
        private readonly static string SP_PREFERENCES = "SP_PREFERENCES";

        #region Get Data

        public List<KIO_AlarmToStoreMessage> GetAlarmToStoreMessage(string storeNo, string alarmNo)
        {
            List<KIO_AlarmToStoreMessage> listAlarm = new List<KIO_AlarmToStoreMessage>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@AlarmNo";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetAlarmToStoreMessage";
                    arrValue[1] = storeNo;
                    arrValue[2] = alarmNo;
                    listAlarm = connection.ExecuteQuery<KIO_AlarmToStoreMessage>(SP_PREFERENCES, arrParam, arrValue).ToList();
                    return listAlarm;
                }
            }
            catch
            {
                return listAlarm;
            }
        }
        public List<KIO_StoreEnvSettings> GetStoreEnvSettings(string storeNo, string envNo)
        {
            List<KIO_StoreEnvSettings> listEnvSettings = new List<KIO_StoreEnvSettings>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[3];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@StoreNo";
                    arrParam[2] = "@EnvNo";
                    object[] arrValue = new object[3];
                    arrValue[0] = "GetStoreEnvSettings";
                    arrValue[1] = storeNo;
                    arrValue[2] = envNo;
                    listEnvSettings = connection.ExecuteQuery<KIO_StoreEnvSettings>(SP_PREFERENCES, arrParam, arrValue).ToList();
                    return listEnvSettings;
                }
            }
            catch
            {
                return listEnvSettings;
            }
        }


        #endregion

        #region Create - Update - Delete

        public Result SaveAlarmStoreMessage(KIO_AlarmToStoreMessage alarm)
        {
            try
            {
                using (var conneciton = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = conneciton.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[13];
                            arrParam[0] = "@Method"; arrParam[1] = "@StoreNo"; arrParam[2] = "@ApprUserRegistAlarm";
                            arrParam[3] = "@ApprOpenDoorAlarm"; arrParam[4] = "@WorkerPhoneNumber"; arrParam[5] = "@TelegramID";
                            arrParam[6] = "@UserRegAlarmMsg"; arrParam[7] = "@OpenDoorAlarmMgt"; arrParam[8] = "@AlarmNo";
                            arrParam[9] = "@TelegramToken"; arrParam[10] = "@ApprUserFaceOkAlarm"; arrParam[11] = "@UserFaceMessage";
                            arrParam[12] = "@SendToMessage";


                            object[] arrValue = new object[13];
                            arrValue[0] = "SaveAlarmStoreMessage"; arrValue[1] = alarm.storeNo; arrValue[2] = alarm.apprUserRegistAlarm;
                            arrValue[3] = alarm.apprOpenDoorAlarm; arrValue[4] = alarm.workerPhoneNumber; arrValue[5] = alarm.telegramId;
                            arrValue[6] = alarm.userRegAlarmMsg; arrValue[7] = alarm.openDoorAlarmMgt; arrValue[8] = alarm.alarmAdminNo;
                            arrValue[9] = alarm.telegramToken; arrValue[10] = alarm.apprUserFaceOkAlarm; arrValue[11] = alarm.userFaceMessage;
                            arrValue[12] = alarm.sendToMessage;


                            resultSave = conneciton.ExecuteScalar<string>(SP_PREFERENCES, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MD0004 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MD0005 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MD0005 };
                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        public Result SaveEnvSettings(KIO_StoreEnvSettings envSettings)
        {
            try
            {
                using (var conneciton = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = conneciton.BeginTransaction())
                    {
                        try
                        {
                            var resultSave = "false";
                            string[] arrParam = new string[11];
                            arrParam[0] = "@Method"; arrParam[1] = "@StoreNo"; arrParam[2] = "@CertifCriteria";
                            arrParam[3] = "@PhoneInput"; arrParam[4] = "@SimilarityRateApproval"; arrParam[5] = "@AuthAfterCompleted";
                            arrParam[6] = "@EnvNo"; arrParam[7] = "@AuthAfterCardId";
                            arrParam[8] = "@EId"; arrParam[9] = "@UseScanner"; arrParam[10] = "@UseCamera";
                            object[] arrValue = new object[11];
                            arrValue[0] = "SaveEnvSettings"; arrValue[1] = envSettings.storeNo; arrValue[2] = envSettings.certifCriteria;
                            arrValue[3] = envSettings.phoneInput;
                            arrValue[4] = envSettings.similarityRateApproval; arrValue[5] = envSettings.authAfterCompleted;
                            arrValue[6] = envSettings.environmentSettingNo; arrValue[7] = envSettings.authAfterCardId;
                            arrValue[8] = envSettings.eId; arrValue[9] = envSettings.useScanner; arrValue[10] = envSettings.useCamera;

                            resultSave = conneciton.ExecuteScalar<string>(SP_PREFERENCES, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MD0004 };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MD0005 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MD0005 };
                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0005 };
            }
        }
        #endregion
    }
}
