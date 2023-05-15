using InfrastructureCore.Helpers;
using InfrastructureCore.Models.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Modules.UIRender.Models
{
    public class MapPostData
    {
        public int MapId { get; set; }

        /// <summary>
        /// GRID/FORM
        /// </summary>
        public string MapType { get; set; }

        /// <summary>
        /// Page Element ID, mappable
        /// </summary>
        public string PageEleId { get; set; }

        /// <summary>
        /// Connection String type, set multiple DB
        /// </summary>
        public string ConnnectionType { get; set; }

        /// <summary>
        /// Dynamic page data from client
        /// </summary>
        public IMapData PostData { get; set; }
    }

    public interface IMapData
    {
        DataSet ToDataSet(Dictionary<string, string> matchParams, SYLoggedUser userInfo);
    }
    public class ParamData
    {
        public string Key { get; set; }
        public dynamic Value { get; set; }
        public string DataType { get; set; }
    }
    public class FormMapData : IMapData
    {
        /// <summary>
        /// INSERT/UPDATE/DELETE
        /// </summary>
        public string FormAction { get; set; }

        /// <summary>
        /// Key = From Field ID = FROM
        /// Value = Value of field;
        /// </summary>
        //public Dictionary<string, object> FormFields { get; set; }
        public List<ParamData> FormFields { get; set; }
        public DataSet ToDataSet(Dictionary<string, string> matchParams, SYLoggedUser userInfo)
        {
            var dsChanges = new DataSet();
            DataTable dtChanges = new DataTable("FormMapData");
            foreach (ParamData field in FormFields)
            {
                dtChanges.Columns.Add(field.Key);
            }
            dsChanges.Tables.Add(dtChanges);

            var nr = dtChanges.NewRow();
            foreach (ParamData field in FormFields)
            {

                if (field.Key.GetType().Name != null)
                {
                    try
                    {
                        if (field.Value != null)
                        {
                            // cast type dynamic value when post to server to save db
                            switch (field.Value.GetType().Name)
                            {
                                // String
                                case "String":
                                    if (field.Value == "" || field.Value == null)
                                    {
                                        nr[field.Key] = DBNull.Value;
                                    }
                                    else
                                    {
                                        nr[field.Key] = field.Value;
                                    }
                                    break;
                                // Number, numeric
                                case "Long":
                                case "Double":
                                case "Int32":
                                case "Int64":
                                case "Decimal":
                                    if (field.Value == 0 || field.Value == null)
                                    {
                                        nr[field.Key] = DBNull.Value;
                                    }
                                    else
                                    {
                                        nr[field.Key] = field.Value;
                                    }
                                    break;
                                // Boolean
                                case "Boolean":
                                    if (field.Value == null)
                                    {
                                        nr[field.Key] = DBNull.Value;
                                    }
                                    else
                                    {
                                        nr[field.Key] = field.Value;
                                    }
                                    break;
                                default:
                                    nr[field.Key] = DBNull.Value;
                                    break;
                            }
                        }
                        else
                        {
                            nr[field.Key] = DBNull.Value;
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Publish(ex, false);
                    }
                    finally
                    {

                    }
                }
            }

            dtChanges.Rows.Add(nr);
            dtChanges.AcceptChanges();
            switch (FormAction)
            {
                case "INSERT":
                    dtChanges.Rows[0].SetAdded(); break;
                case "UPDATE":
                    dtChanges.Rows[0].SetModified(); break;
                case "DELETE":
                    dtChanges.Rows[0].Delete(); break;
                default:
                    break;
            }

            return dsChanges;
        }
        //public DataSet ToDataSet(Dictionary<string, string> matchParams)
        //{
        //    var dsChanges = new DataSet();
        //    DataTable dtChanges = new DataTable("FormMapData");
        //    foreach (string field in FormFields.Keys)
        //    {
        //        dtChanges.Columns.Add(matchParams[field]);
        //    }
        //    dsChanges.Tables.Add(dtChanges);

        //    var nr = dtChanges.NewRow();            
        //    foreach (string field in FormFields.Keys)
        //    {
        //        nr[matchParams[field]] = FormFields[field];
        //    }

        //    dtChanges.Rows.Add(nr);
        //    dtChanges.AcceptChanges();
        //    switch (FormAction)
        //    {
        //        case "INSERT":
        //            dtChanges.Rows[0].SetAdded(); break;
        //        case "UPDATE":
        //            dtChanges.Rows[0].SetModified(); break;
        //        case "DELETE":
        //            dtChanges.Rows[0].Delete(); break;
        //        default:
        //            break;
        //    }

        //    return dsChanges;
        //}
    }

    public class GridMapData : IMapData
    {
        /// <summary>
        /// Primary key for grid
        /// </summary>
        public string[] RowKeys { get; set; }
        public Dictionary<string, object>[] AddedRows { get; set; }
        public Dictionary<string, object>[] UpdatedRows { get; set; }
        public Dictionary<string, object>[] DeletedRows { get; set; }

        public DataSet ToDataSet(Dictionary<string, string> matchParams, SYLoggedUser userInfo)
        {
            var dsChanges = new DataSet();
            DataTable dtChanges = new DataTable("GridMapData");

            var hasColumns = AddedRows.Length > 0 ? AddedRows : (UpdatedRows.Length > 0 ? UpdatedRows : (DeletedRows.Length > 0 ? DeletedRows : null));
            if (hasColumns != null)
            {
                foreach (string field in hasColumns[0].Keys)
                {
                    if (matchParams.ContainsKey(field))
                    {
                        dtChanges.Columns.Add(matchParams[field]);
                    }
                }
                // mapping with column in table design in DB
                dtChanges.Columns.Add("Created_By");
                dtChanges.Columns.Add("Updated_By");

                dsChanges.Tables.Add(dtChanges);

                if (AddedRows != null)
                {
                    for (int i = 0; i < AddedRows.Length; i++)
                    {
                        var nr = dtChanges.NewRow();
                        foreach (string field in hasColumns[0].Keys)
                        {
                            if (!matchParams.ContainsKey(field)) continue;
                            if(AddedRows[i][field].ToString() =="")
                            {
                                nr[matchParams[field]] = DBNull.Value;
                            }
                            else
                            {
                                //nr[matchParams[field]] = AddedRows[i][field];
                                // cast type dynamic value when post to server to save db
                                switch (AddedRows[i][field].GetType().Name)
                                {
                                    // String
                                    case "String":
                                    case "string":
                                        nr[matchParams[field]] = AddedRows[i][field].ToString().Trim();
                                        break;
                                    default:
                                        nr[matchParams[field]] = AddedRows[i][field];
                                        break;
                                }
                            }
                        }
                        nr["Created_By"] = userInfo.UserID;
                        nr["Updated_By"] = userInfo.UserID;

                        dtChanges.Rows.Add(nr);
                    }
                }

                if (UpdatedRows != null)
                {
                    for (int i = 0; i < UpdatedRows.Length; i++)
                    {
                        var nr = dtChanges.NewRow();
                        foreach (string field in hasColumns[0].Keys)
                        {
                            //if (!matchParams.ContainsKey(field)) continue;
                            //nr[matchParams[field]] = UpdatedRows[i][field];

                            if (!matchParams.ContainsKey(field)) continue;
                            //if (UpdatedRows[i][field].ToString() == "")
                           if (UpdatedRows[i][field] == "" || UpdatedRows[i][field] == null)
                            {
                                nr[matchParams[field]] = DBNull.Value;
                            }
                            else
                            {
                                //nr[matchParams[field]] = UpdatedRows[i][field];
                                // cast type dynamic value when post to server to save db
                                switch (UpdatedRows[i][field].GetType().Name)
                                {
                                    // String
                                    case "String":
                                    case "string":
                                        nr[matchParams[field]] = UpdatedRows[i][field].ToString().Trim();
                                        break;
                                    default:
                                        nr[matchParams[field]] = UpdatedRows[i][field];
                                        break;
                                }
                            }
                        }

                        nr["Updated_By"] = userInfo.UserID;
                        dtChanges.Rows.Add(nr);
                        nr.AcceptChanges();
                        nr.SetModified();
                    }
                }

                if (DeletedRows != null)
                {
                    for (int i = 0; i < DeletedRows.Length; i++)
                    {
                        var nr = dtChanges.NewRow();
                        foreach (string field in hasColumns[0].Keys)
                        {
                            if (!matchParams.ContainsKey(field)) continue;
                            nr[matchParams[field]] = DeletedRows[i][field];
                        }
                        nr["Updated_By"] = userInfo.UserID;
                        dtChanges.Rows.Add(nr);
                        nr.AcceptChanges();
                        nr.Delete();
                    }
                }
            }

            return dsChanges;
        }
    }

    public class MapDataSource
    {
        public string DSName { get; set; }
        public Dictionary<string, string> MapFields { get; set; }
        public Dictionary<string, string> MapFieldsDatatype { get; set; }
        public Dictionary<string, int> MapFieldsDataSize { get; set; }
        public MapPostData MapData { get; set; }
    }
}
