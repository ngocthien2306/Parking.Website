using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Kiosk.Management.Repositories.IRepository;
using Modules.Pleiger.CommonModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Kiosk.Management.Repositories.Repository
{
    public class KIAVoiceFileRepository : IKIOVoiceFileRepository
    {
        private readonly string SP_VOICE_FILE_MANAGEMENT = "SP_VOICE_FILE_MANAGEMENT";
        #region Get Data 
        public List<KIO_ClientSoundMgt> GetListAudioFile(string id, string name)
        {
            try
            {
                List<KIO_ClientSoundMgt> soundMgts = new List<KIO_ClientSoundMgt>();
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@SoundId";
                    arrParams[2] = "@SoundName";
                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "GetListAudioFile";
                    arrParamsValue[1] = id;
                    arrParamsValue[2] = name;
                    var result = connection.ExecuteQuery<KIO_ClientSoundMgt>(SP_VOICE_FILE_MANAGEMENT, arrParams, arrParamsValue);
                    soundMgts = result.ToList();
                    return soundMgts;
                }
            }
            catch
            {
                return null;
            }
        }
        public List<KIO_StoreDeployHistory> GetSoundDeployHist(string soundId)
        {
            List<KIO_StoreDeployHistory> deployHistories = new List<KIO_StoreDeployHistory>();
            try
            {
                using (var connection = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@SoundId";

                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "GetSoundDeployHist";
                    arrParamsValue[1] = soundId;
              
                    var result = connection.ExecuteQuery<KIO_StoreDeployHistory>(SP_VOICE_FILE_MANAGEMENT, arrParams, arrParamsValue);
                    deployHistories = result.ToList();
                    return deployHistories;
                }
            }
            catch
            {
                return deployHistories;
            }
        }
        #endregion Get Data

        #region Create - Update - Delete
        public Result DeleteAudioFile(string soundId)
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
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method";
                            arrParam[1] = "@SoundId";
         
                            object[] arrValue = new object[2];
                            arrValue[0] = "DeleteAudioFile";
                            arrValue[1] = soundId;

                            resultDelete = connection.ExecuteScalar<string>(SP_VOICE_FILE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
                            transaction.Commit();
                            if (resultDelete != "false")
                            {
                                return new Result { Success = true, Message = MessageCode.MD0008, Data = resultDelete };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result { Success = false, Message = MessageCode.MD0015 };
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            return new Result { Success = false, Message = MessageCode.MD0015 };

                        }
                    }
                }
            }
            catch
            {
                return new Result { Success = false, Message = MessageCode.MD0015 };
            }
        }

        public Result SaveAudioFile(KIO_ClientSoundMgt clientSoundMgt, string userId)
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
                            string[] arrParam = new string[8];
                            arrParam[0] = "@Method"; arrParam[1] = "@SoundId"; arrParam[2] = "@SoundName";
                            arrParam[3] = "@LocalFileLocation"; arrParam[4] = "@UserId";
                            arrParam[5] = "@KioskFolderLocation"; arrParam[6] = "@DeployStatus";
                            arrParam[7] = "@SoundType";
                            object[] arrValue = new object[8];
                            arrValue[0] = "SaveAudioFile"; arrValue[1] = clientSoundMgt.soundNo; 
                            arrValue[2] = clientSoundMgt.soundName;
                            arrValue[3] = clientSoundMgt.localFileLocation;
                            arrValue[4] = userId; 
                            arrValue[5] = clientSoundMgt.kioskFolderLocation == null ? "" : clientSoundMgt.kioskFolderLocation;
                            arrValue[6] = clientSoundMgt.deployStatus; arrValue[7] = clientSoundMgt.soundType;


                            resultSave = conneciton.ExecuteScalar<string>(SP_VOICE_FILE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
        public Result UpdateVersionAudio(string soundNo)
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
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method"; 
                            arrParam[1] = "@SoundId";
                            object[] arrValue = new object[2];
                            arrValue[0] = "UpdateVersionAudio"; 
                            arrValue[1] = soundNo;
                            resultSave = conneciton.ExecuteScalar<string>(SP_VOICE_FILE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
        public Result SaveTempDeploy(string storeNo, string deviceNo, string soundNo, byte[] source)
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
                            string[] arrParam = new string[5];
                            arrParam[0] = "@Method"; arrParam[1] = "@SoundId"; arrParam[2] = "@DeviceNo";
                            arrParam[3] = "@StoreNo"; arrParam[4] = "@Source";
                   
                            object[] arrValue = new object[5];
                            arrValue[0] = "SaveTempDeploy"; arrValue[1] = soundNo;
                            arrValue[2] = deviceNo;
                            arrValue[3] = storeNo;
                            arrValue[4] = source;
      

                            resultSave = conneciton.ExecuteScalar<string>(SP_VOICE_FILE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
        public Result UpdateTempDeploy(string soundNo)
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
                            string[] arrParam = new string[2];
                            arrParam[0] = "@Method"; arrParam[1] = "@SoundId";

                            object[] arrValue = new object[2];
                            arrValue[0] = "UpdateTempDeploy"; arrValue[1] = soundNo;


                            resultSave = conneciton.ExecuteScalar<string>(SP_VOICE_FILE_MANAGEMENT, CommandType.StoredProcedure, arrParam, arrValue, transaction);
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
