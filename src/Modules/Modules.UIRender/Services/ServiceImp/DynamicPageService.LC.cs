using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using Modules.UIRender.Models;
using Modules.UIRender.Services.IService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using InfrastructureCore.DAL.Engines;
using System.Data.SqlClient;
using InfrastructureCore.Configuration;
using System.Data.Common;
using InfrastructureCore.Models.Identity;

namespace Modules.UIRender.Services.ServiceImp
{
    public partial class DynamicPageService : IDynamicPageService
    {
        public object ExecuteSave(string postJsonData, SYLoggedUser userInfo)
        {
            MapPostData[] mapDatas = ParsePostData(postJsonData);

            bool result = false;
            string errorMessage = string.Empty;

            // processing saving to db
            if (mapDatas != null && mapDatas.Length > 0)
            {
                result = SaveMapDatas(mapDatas, out errorMessage, userInfo);
            }

            return new
            {
                Success = result,
                ErrorMessage = errorMessage
            };
        }

        #region Utils

        private MapPostData[] ParsePostData(string postJsonStr)
        {
            dynamic postDataObj = JsonConvert.DeserializeObject(postJsonStr);
            List<MapPostData> retPostDatas = new List<MapPostData>();
            List<dynamic> mapDatas = new List<dynamic>();
            if (postDataObj is JArray)
            {
                mapDatas = JsonConvert.DeserializeObject<List<dynamic>>(postJsonStr);
            }
            else
            {
                mapDatas.Add(postDataObj);
            }

            for (int i = 0; i < mapDatas.Count; i++)
            {
                MapPostData mpd = new MapPostData()
                {
                    MapId = mapDatas[i].MapId,
                    MapType = mapDatas[i].MapType,
                    PageEleId = mapDatas[i].PelId,
                    ConnnectionType = mapDatas[i].ConnnectionType
                };

                var subData = mapDatas[i].PostData;
                switch (mpd.MapType)
                {
                    case "GRID":
                        // PostData is GridMapData
                        mpd.PostData = JsonConvert.DeserializeObject<GridMapData>(JsonConvert.SerializeObject(subData));
                        break;
                    case "FORM":
                        // PostData is FormMapData
                        mpd.PostData = JsonConvert.DeserializeObject<FormMapData>(JsonConvert.SerializeObject(subData));
                        break;
                    default:
                        break;
                }

                retPostDatas.Add(mpd);
            }

            return retPostDatas.ToArray();
        }

        /// <summary>
        /// TO DO
        /// 1. EACH MapPostData, 
        ///     - Get SP & Param names
        ///     - Generate DataSet;
        ///     - Run DACHelper
        /// 
        /// </summary>
        /// <param name="postDatas"></param>
        /// <returns></returns>
        private bool SaveMapDatas(MapPostData[] postDatas, out string errorMessage, SYLoggedUser userInfo)
        {
            const string GRID_ERROR_MESSAGE = "Grid data is not changed.";
            errorMessage = string.Empty;
            var res = new Result();
            List<MapDataSource> mapDataSources = new List<MapDataSource>();
            for (int i = 0; i < postDatas.Length; i++)
            {
                mapDataSources.Add(FetchMapDS(postDatas[i]));
            }
            DbConnectionInfo dbConnectionInfo = new DbConnectionInfo();
            /// Cannot set transaction when for mapDataSources
            for (int i = 0; i < mapDataSources.Count; i++)
            {
                string connnectionType = mapDataSources[i].MapData.ConnnectionType;
                if (connnectionType != null)
                {
                    switch (connnectionType)
                    {
                        case "G013C000":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.FrameworkConnection;
                            break;
                        case "G013C001":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.DbConnection1;
                            break;
                        case "G013C002":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.DbConnection2;
                            break;
                        case "G013C003":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.DbConnection3;
                            break;
                        case "G013C004":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.DbConnection4;
                            break;
                        case "G013C005":
                            dbConnectionInfo = GlobalConfiguration.DbConnections.DbConnection5;
                            break;
                    }
                }
            }

            using (var dac = DataConnectionFactory.GetConnection(dbConnectionInfo))
            {
                DbTransaction trans = null;
                /// Cannot set transaction when for mapDataSources
                if (dbConnectionInfo != null) // FrameWork
                {
                    DbmsTypes DbType = dbConnectionInfo.DbType;
                    switch (DbType)
                    {
                        case DbmsTypes.Mssql:
                            trans = dac.BeginTransaction();
                            break;
                        case DbmsTypes.MySql:
                            trans = dac.BeginMySqlTransaction();
                            break;
                        case DbmsTypes.Oracle: break; // not implement
                        case DbmsTypes.MariaDb: break; // not implement
                        case DbmsTypes.PostgreSql: break; // not implement
                        case DbmsTypes.Sqlite: break; // not implement
                    }
                }

                try
                {
                    for (int i = 0; i < mapDataSources.Count; i++)
                    {
                        var mds = mapDataSources[i];
                        if (mds == null)
                        {
                            continue;
                        }
                        var dsChanged = mds.MapData.PostData.ToDataSet(mds.MapFields, userInfo);
                        if (dsChanged.Tables.Count != 0)
                        {
                            res = dac.ExecuteUpdateData(dsChanged, mds.DSName,
                                mds.MapFields.Values.ToArray("@"), mds.MapFieldsDatatype.Values.ToArray(), mds.MapFieldsDataSize.Values.ToArray(),
                                dsChanged.Tables[0].TableName, trans);
                            if (!res.Success)
                            {
                                // Case Fail
                                break;
                            }
                        }
                        else
                        {
                            errorMessage = GRID_ERROR_MESSAGE;
                            continue;
                        }
                    }
                    // Case fail
                    if (!res.Success)
                    {
                        errorMessage = res.Message;// "execute data error.";
                        trans.Rollback();
                    }
                    else
                    {
                        // Case successful
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    trans.Rollback();
                }
                finally
                {
                    if (trans != null)
                    {
                        trans.Dispose();
                    }
                }
            }
            // Case form and grid in one page
            if (res.Success && errorMessage == GRID_ERROR_MESSAGE)
            {
                errorMessage = "";
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        /// <summary>
        /// Search in db, 
        /// find Mapping data source info
        /// </summary>
        /// <param name="mpd"></param>
        /// <returns></returns>
        private MapDataSource FetchMapDS(MapPostData mpd)
        {
            using (var dac = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var ds = dac.ExecuteQuery("SP_Web_SYGetDataMapInfo",
                    new string[] { "@MAP_ID" },
                    new object[] { mpd.MapId });

                if (ds != null && ds.Tables.Count > 1 &&
                    ds.Tables[0].Rows.Count > 0 && ds.Tables[1].Rows.Count > 0)
                {
                    var mds = new MapDataSource()
                    {
                        DSName = ds.Tables[0].Rows[0]["MAP_SPNM"].ToString(),
                        MapFields = new Dictionary<string, string>(),
                        MapFieldsDatatype = new Dictionary<string, string>(),
                        MapFieldsDataSize = new Dictionary<string, int>(),
                        MapData = mpd
                    };

                    foreach (DataRow dataRow in ds.Tables[1].Rows)
                    {
                        mds.MapFields.Add(dataRow["FromField"].ToString(), dataRow["ToDSParam"].ToString());
                        mds.MapFieldsDatatype.Add(dataRow["ToDSParam"].ToString(), dataRow["DataType"].ToString());
                        mds.MapFieldsDataSize.Add(dataRow["ToDSParam"].ToString(), dataRow["ParamSize"] != DBNull.Value ? Convert.ToInt32(dataRow["ParamSize"]) : 0);
                    }

                    //mds.MapFields.Add("Created_By", "Created_By");
                    //mds.MapFields.Add("Updated_By", "Updated_By");
                    //mds.MapFieldsDatatype.Add("Created_By", "Char");
                    //mds.MapFieldsDatatype.Add("Updated_By", "Char");
                    //mds.MapFieldsDataSize.Add("Created_By", 0);
                    //mds.MapFieldsDataSize.Add("Updated_By", 0);
                    return mds;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }
}
