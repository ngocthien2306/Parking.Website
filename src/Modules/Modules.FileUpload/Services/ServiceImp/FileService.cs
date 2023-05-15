using Modules.FileUpload.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using InfrastructureCore.DAL;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Modules.Common.Models;

namespace Modules.FileUpload.Services.ServiceImp
{
    public class FileService : IFileService
    {
        IDBContextConnection dbConnection;

        public FileService(IDBContextConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }
        public List<SYFileUpload> GetListSYFileUpload()
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    var files = conn.ExecuteQuery<SYFileUpload>("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV" },
            //        new object[] { "SELECT" });
            //    return files.ToList();
            //}

            var dataMapsDl = new List<SYFileUpload>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "SELECT");
                    var rs = SqlMapper.Query<SYFileUpload>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public SYFileUpload GetSYFileUploadByID(string fileGuid)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    var files = conn.ExecuteQuery<SYFileUpload>("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileGuid" },
            //        new object[] { "SELECTBYID", fileGuid });
            //    return files.FirstOrDefault();
            //}

            var dataMapsDl = new SYFileUpload();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "SELECTBYID");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, fileGuid);
                    var rs = SqlMapper.Query<SYFileUpload>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.FirstOrDefault();
                }
                conn.Close();
            }
            catch 
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public List<SYFileUpload> GetListSYFileUploadByID(string fileGuid)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    var files = conn.ExecuteQuery<SYFileUpload>("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileGuid" },
            //        new object[] { "SELECTBYID", fileGuid });
            //    return files.FirstOrDefault();
            //}

            var dataMapsDl = new List<SYFileUpload>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "SELECTBYFILEID");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, fileGuid);
                    var rs = SqlMapper.Query<SYFileUpload>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public int InsertSYFileUpload(SYFileUpload file)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    return conn.ExecuteNonQuery("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileGuid", "@FileName", "@FileNameSave", "@FileType", "@FileSize" },
            //        new object[] { "INSERT", file.FileGuid, file.FileName, file.FileNameSave, file.FileType, file.FileSize });
            //}

            var result = -1;
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "INSERT");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, file.FileGuid);
                    dyParam.Add("@FileName", SqlDbType.NVarChar, ParameterDirection.Input, file.FileName);
                    dyParam.Add("@FileNameSave", SqlDbType.NVarChar, ParameterDirection.Input, file.FileNameSave);
                    dyParam.Add("@FileType", SqlDbType.NVarChar, ParameterDirection.Input, file.FileType);
                    // Thien add URL 2022-01-06
                    dyParam.Add("@URL", SqlDbType.NVarChar, ParameterDirection.Input, file.URL);
                    dyParam.Add("@FileSize", SqlDbType.Int, ParameterDirection.Input, file.FileSize);
                    //Quan add siteID 2021-01-16
                    dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, file.SiteID);

                    result = SqlMapper.Execute(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
                conn.Close();
            }
            catch 
            {
                conn.Close();
            }
            return result;
        }
        public int UpdateSYFileUpload(SYFileUpload file)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    return conn.ExecuteNonQuery("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileGuid", "@FileName", "@FileNameSave", "@FileType", "@FileSize" },
            //        new object[] { "UPDATE", file.FileGuid, file.FileName, file.FileNameSave, file.FileType, file.FileSize });
            //}

            var result = -1;
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "UPDATE");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, file.FileGuid);
                    dyParam.Add("@FileName", SqlDbType.NVarChar, ParameterDirection.Input, file.FileName);
                    dyParam.Add("@FileNameSave", SqlDbType.NVarChar, ParameterDirection.Input, file.FileNameSave);
                    dyParam.Add("@FileType", SqlDbType.NVarChar, ParameterDirection.Input, file.FileType);
                    dyParam.Add("@FileSize", SqlDbType.Int, ParameterDirection.Input, file.FileSize);

                    result = SqlMapper.Execute(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return result;
        }
        public int DeleteSYFileUpload(string fileGuid, string spName, string fileMst, string BoardDocID)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    return conn.ExecuteNonQuery("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileGuid" },
            //        new object[] { "DELETE", fileGuid });
            //}

            var result = -1;
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "DELETE");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, fileGuid);
                    result = SqlMapper.Execute(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
                UpdateFileID(spName, fileMst, BoardDocID);
                conn.Close();
            }
            catch
            {
                conn.Close();
            }
            return result;
        }

        public int InsertSYFileUploadMaster(string fileID, string fileGuid, string filePath, int siteID)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    return conn.ExecuteNonQuery("SP_HLC_SYFileUpload",
            //        new string[] { "@DIV", "@FileID", "@FileGuid" },
            //        new object[] { "INSERTMASTER", fileID, fileGuid });
            //}
            var result = -1;
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "INSERTMASTER");
                    dyParam.Add("@FileID", SqlDbType.NVarChar, ParameterDirection.Input, fileID);
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, fileGuid);
                    dyParam.Add("@FilePath", SqlDbType.NVarChar, ParameterDirection.Input, filePath);
                    dyParam.Add("@SiteID", SqlDbType.NVarChar, ParameterDirection.Input, siteID);                  
                    result = SqlMapper.Execute(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return result;
        }
        public List<SYFileUploadMaster> GetSYFileUploadMasterByID(string fileId)
        {
            var dataMapsDl = new List<SYFileUploadMaster>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "SELECTMASTERBYID");
                    dyParam.Add("@FileID", SqlDbType.NVarChar, ParameterDirection.Input, fileId);
                    var rs = SqlMapper.Query<SYFileUploadMaster>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public SYFileUploadMaster GetSYFileUploadMasterByFileGuid(string fileGuid)
        {
            var dataMapsDl = new SYFileUploadMaster();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_HLC_SYFileUpload";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "SELECTMASTERBYFILEGUID");
                    dyParam.Add("@FileGuid", SqlDbType.NVarChar, ParameterDirection.Input, fileGuid);
                    var rs = SqlMapper.Query<SYFileUploadMaster>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.FirstOrDefault();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public Result SaveFile(IFormFile file, string destinationPath, string directory)
        {
            var result = new Result();

            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                using (var stream = new FileStream(destinationPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                result.Success = true;
                result.Message = "Upload file successful!";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
                return result;
            }
        }
        public Result InsertSYFileUploadPartner(string fileID, string fileGuid, string spName, string pagID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SAVE_FILE_DATA_GRID",
                        new string[] { "@DIV", "@fileID", "@pagID" },
                        new object[] { spName, fileID, pagID }
                    );
                    if (result > 0)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Save data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Save data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }
        public Result UpdateFileID(string spName, string fileMst, string BoardDocID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("CHECK_FILE_DELETED",
                        new string[] { "@spName", "@fileID", "@BoardDocID" },
                        new object[] { spName, fileMst, BoardDocID }
                    );
                    if (result > 0)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Save data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Save data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }
    }
}
