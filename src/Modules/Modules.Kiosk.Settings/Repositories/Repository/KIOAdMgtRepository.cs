using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Kiosk.Settings.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Settings.Repositories.Repository
{
    public class KIOAdMgtRepository : IKIOAdMgtRepository
    {
        private readonly static string SP_AD_MANAGEMENT = "SP_AD_MANAGEMENT";

        #region Get Data
        public List<KIO_AdMgt> GetAdMgt(KIO_AdMgt adMgt)
        {
            List<KIO_AdMgt> listAd = new List<KIO_AdMgt>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[7];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@AdNo";
                    arrParam[2] = "@AdName";
                    arrParam[3] = "@PeriodStartDate";
                    arrParam[4] = "@PeriodEndDate";
                    arrParam[5] = "@AdStatus";
                    arrParam[6] = "@AdType";
                    object[] arrValue = new object[7];
                    arrValue[0] = "GetAdMgt";
                    arrValue[1] = adMgt.adNo;
                    arrValue[2] = adMgt.adName;
                    arrValue[3] = adMgt.periodStartDate;
                    arrValue[4] = adMgt.periodEndDate;
                    arrValue[5] = adMgt.adStatus;
                    arrValue[6] = adMgt.adType;
                    listAd = connection.ExecuteQuery<KIO_AdMgt>(SP_AD_MANAGEMENT, arrParam, arrValue).ToList();
                    foreach(var ad in listAd)
                    {
                        ad.dayTime = Convert.ToDateTime(ad.dayStartTime).ToString("HH:mm") + " ~ " + Convert.ToDateTime(ad.dayEndTime).ToString("HH:mm");//Convert.ToDateTime(ad.periodEndDate).Subtract(Convert.ToDateTime(ad.periodStartDate));
                        ad.period = Convert.ToDateTime(ad.periodStartDate).ToString("yyyy-MM-dd") + " ~ " + Convert.ToDateTime(ad.periodEndDate).ToString("yyyy-MM-dd");
                    }
                    return listAd;
                }
            }
            catch
            {
                return listAd;
            }
        }
        public List<KIO_StoreMaster> GetAdMgtStore(string adNo)
        {
            List<KIO_StoreMaster> listStoreMaster = new List<KIO_StoreMaster>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@AdNo";
                    object[] arrValue = new object[2];
                    arrValue[0] = "GetAdMgtStore";
                    arrValue[1] = adNo;
                    listStoreMaster = connection.ExecuteQuery<KIO_StoreMaster>(SP_AD_MANAGEMENT, arrParam, arrValue).ToList();
                    listStoreMaster.ForEach(d => d.no = 0);
                    return listStoreMaster;
                }
            }
            catch
            {
                return listStoreMaster;
            }
        }

        #endregion

        #region Create - Update - Delete
        public Result DeleteAdMgt(string adNo, bool checkDeleteAd)
        {
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var resultDelete = "false";
                            string[] arrParam = new string[3];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@AdNo";
                            arrParam[2] = "@CheckDeleteAd";
                            object[] arrValue = new object[3];
                            arrValue[0] = "DeleteAdMgt";
                            arrValue[1] = adNo;
                            arrValue[2] = checkDeleteAd;
                            resultDelete = connection.ExecuteScalar<string>(SP_AD_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete == "1")
                            {
                                return new Result { Success = true, Message = MessageCode.MEAD03 };
                            }
                            else
                            {
                                var message = resultDelete == MessageCode.conflictKeyMessage ? MessageCode.conflictChangeMessage : resultDelete == "0" ? MessageCode.MEAD04 : MessageCode.conflictChangeMessage;
                                //transaction.Rollback();
                                return new Result { Success = false, Message = message };
                            }
                        }
                        catch
                        {
                            //transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MEAD04 };

                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MEAD04 };
            }
        }

        public Result SaveAdMgt(KIO_AdMgt adMgt, string listStoreNo)
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
                            arrParam[0] = "@Method"; arrParam[1] = "@AdType"; arrParam[2] = "@AdName";
                            arrParam[3] = "@PeriodStartDate"; arrParam[4] = "@PeriodEndDate"; arrParam[5] = "@DayStartTime";
                            arrParam[6] = "@DayEndTime"; arrParam[7] = "@AdStatus"; arrParam[8] = "@ResitUser";
                            arrParam[9] = "@AdNo"; arrParam[10] = "@AdLocation"; arrParam[11] = "@AttachFilePath";
                            arrParam[12] = "@ListStoreNo";
                            object[] arrValue = new object[13];
                            arrValue[0] = "SaveAdMgt"; arrValue[1] = adMgt.adType; arrValue[2] = adMgt.adName;
                            arrValue[3] = adMgt.periodStartDate?.ToString("yyyy-MM-dd HH:mm:ss"); 
                            arrValue[4] = adMgt.periodEndDate?.ToString("yyyy-MM-dd HH:mm:ss"); 
                            arrValue[5] = adMgt.dayStartTime?.ToString("yyyy-MM-dd HH:mm:ss");
                            arrValue[6] = adMgt.dayEndTime?.ToString("yyyy-MM-dd HH:mm:ss");
                            arrValue[7] = adMgt.adStatus; arrValue[8] = adMgt.resitUser;
                            arrValue[9] = adMgt.adNo; arrValue[10] = adMgt.adLocation; arrValue[11] = adMgt.attachFilePath;
                            arrValue[12] = listStoreNo;

                            resultSave = conneciton.ExecuteScalar<string>(SP_AD_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultSave != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MEAD01, Data=resultSave};
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MEAD02 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MEAD02 };
                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MEAD02 };
            }
        }


        #endregion
    }
}
