using HLCoreFX.CoreLib.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace HLCoreFX.CoreLib
{
    public class FXLayoutManager
    {
        /// <summary>
        /// Datasource for mapping connection string
        /// </summary>
        public static string DSMAP_CS = "Data Source=mssql.thlsoft.com,1433;Initial Catalog=LzDxpBase;User ID=thlsoft;Password=123456789a;Persist Security Info=True;";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postJsonData"></param>
        public static object SavePostData(string postJsonData)
        {
            MapPostData[] mapDatas = ParsePostData(postJsonData);

            bool result = false;
            string errorMessage = string.Empty;

            // processing saving to db
            if (mapDatas != null && mapDatas.Length > 0)
            {
                result = SaveMapDatas(mapDatas, out errorMessage);
            }

            return new
            {
                Success = result,
                ErrorMessage = errorMessage
            };
        }

        #region Utils

        private static MapPostData[] ParsePostData(string postJsonStr)
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
                    PageEleId = mapDatas[i].PelId
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
        private static bool SaveMapDatas(MapPostData[] postDatas, out string errorMessage)
        {
            var rs = new ResultMessage();
            errorMessage = string.Empty;
            List<MapDataSource> mapDataSources = new List<MapDataSource>();
            for (int i = 0; i < postDatas.Length; i++)
            {
                mapDataSources.Add(FetchMapDS(postDatas[i]));
            }

            using (var dac = new DACHelper(DSMAP_CS))
            {
                var trans = dac.BeginTransaction();
                try
                {                   
                    for (int i = 0; i < mapDataSources.Count; i++)
                    {
                        var mds = mapDataSources[i];
                        if(mds == null)
                        {
                            continue;
                        }
                        var dsChanged = mds.MapData.PostData.ToDataSet(mds.MapFields);
                        if (dsChanged.Tables.Count != 0)
                        {
                            //rs = dac.ExecuteUpdateData(dsChanged, mds.DSName,
                            //mds.MapFields.Values.ToArray("@"),
                            //dsChanged.Tables[0].TableName, trans);

                            // rs = dac.ExecuteUpdateData2(ref dsChanged, mds.DSName, mds.MapFields.Values.ToArray("@"), dsChanged.Tables[0].TableName, trans);
                            rs = dac.ExecuteUpdateData(dsChanged, mds.DSName,
                                mds.MapFields.Values.ToArray("@"), mds.MapFieldsDatatype.Values.ToArray(), mds.MapFieldsDataSize.Values.ToArray(),
                                //StringArrToIntArr(mds.MapFieldsDataSize.Values.ToArray()),
                                dsChanged.Tables[0].TableName, trans);
                            if (rs.result == -1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            errorMessage = "grid not changed.";
                            continue;
                        }
                        
                    }
                    if (rs.result == -1)
                    {
                        errorMessage = rs.message;// "execute data error.";
                        trans.Rollback();
                    }
                    else
                    {
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
            if(rs.result == 1 && errorMessage == "grid not changed.")
            {
                errorMessage = "";
            }
            return string.IsNullOrEmpty(errorMessage);
        }

        public static int[] StringArrToIntArr(String[] s)
        {
            int[] result = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                result[i] = Int32.Parse(s[i]);
            }
            return result;
        }

        /// <summary>
        /// Search in db, 
        /// find Mapping data source info
        /// </summary>
        /// <param name="mpd"></param>
        /// <returns></returns>
        private static MapDataSource FetchMapDS(MapPostData mpd)
        {
            using (var dac = new DACHelper(DSMAP_CS))
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
                        mds.MapFieldsDataSize.Add(dataRow["ToDSParam"].ToString(), dataRow["ParamSize"] != DBNull.Value ? Convert.ToInt32(dataRow["ParamSize"]) : 0 );
                    }

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
