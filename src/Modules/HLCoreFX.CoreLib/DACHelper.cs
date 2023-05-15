using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HLCoreFX.CoreLib
{
    public class DACHelper : DacBase
    {
        public static string DefaultConnectionString = string.Empty;
        private const int INTERVAL_SCHEDULE = 3000;
        private const int CONN_TIMES = 3;

        /// <summary>
        /// Constructor, provide SubSystemType to indicate which ConnectionString in machine.config 
        /// to use
        /// </summary>
        /// <param name="susSystemType"></param>
        public DACHelper()
            : base(DefaultConnectionString)
        {
        }

        public DACHelper(string connectionString)
            : base(connectionString)
        {

        }

        #region ExecuteQuery

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryWithProgressBar(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteQueryWithProgressBar(600, storedProcName, paramNames, paramValues);
        }

        public DataSet ExecuteQueryWithProgressBar(int timeOut, string storedProcName, string[] paramNames, object[] paramValues)
        {
            DataSet result = null;
            ExecuteOptimize();
            result = ExecuteQuery(timeOut, CommandType.StoredProcedure, storedProcName, paramNames, paramValues, null);
            return result;
        }

        /// <summary>
        /// Added on 01/23 Loc
        /// </summary>
        private void ExecuteOptimize()
        {
            SetSqlCommand(SqlCommandType.SelectCommand, "SET ARITHABORT ON", CommandType.Text);
            ExecuteNonQuery(false);
        }

        /// <summary>
        /// Return Dataset with 1 table
        /// </summary>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(120, CommandType.StoredProcedure, storedProcName, paramNames, paramValues, null);
        }

        public DataSet ExecuteQuery(string storedProcName, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            return ExecuteQuery(120, CommandType.StoredProcedure, storedProcName, paramNames, paramValues, trans);
        }

        public DataSet ExecuteQueryText(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(120, CommandType.Text, storedProcName, paramNames, paramValues, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeOutInSeconds"></param>
        /// <param name="commandType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(int timeOutInSeconds, CommandType commandType, string storedProcName,
            string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(timeOutInSeconds, commandType, storedProcName, paramNames, paramValues, null);
        }

        /// <summary>
        /// Return Dataset with 1 table
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(int timeOutInSeconds, CommandType commandType, string storedProcName,
            string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            DataSet dsReturn = null;
            DateTime startExecute = DateTime.Now;

            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    SqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            loadCommand.CommandTimeout = timeOutInSeconds;
            if (trans != null)
            {
                loadCommand.Transaction = trans;
            }
            dsReturn = ExecuteFill();
            return dsReturn;
        }

        #endregion

        //private void Log

        #region ExecuteQueryDataRow

        /// <summary>
        /// Return a single Row or null
        /// </summary>
        /// <param name="subSystemType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataRow ExecuteQueryDataRow(string storedProcName, string[] paramNames, object[] paramValues)
        {
            DataSet ds = ExecuteQuery(storedProcName, paramNames, paramValues);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        /// <summary>
        /// Return a single Row or null
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="subSystemType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataRow ExecuteQueryDataRow(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues)
        {
            DataSet ds = ExecuteQuery(timeOutInSeconds, CommandType.StoredProcedure, storedProcName, paramNames, paramValues, null);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        #endregion

        #region ExecuteNonQuery

        public void ExecuteNonQueryText(string storedProcName, string[] paramNames, object[] paramValues)
        {
            ExecuteNonQuery(120, storedProcName, CommandType.Text, paramNames, paramValues, false);
        }

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(string storedProcName, string[] paramNames, object[] paramValues)
        {
            ExecuteNonQuery(120, storedProcName, paramNames, paramValues, false);
        }

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues)
        {
            ExecuteNonQuery(timeOutInSeconds, storedProcName, paramNames, paramValues, false);
        }

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(string storedProcName, string[] paramNames, object[] paramValues, bool transactionYN)
        {
            ExecuteNonQuery(120, storedProcName, paramNames, paramValues, transactionYN);
        }

        public void ExecuteNonQuery(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues, bool transactionYN)
        {
            ExecuteNonQuery(timeOutInSeconds, storedProcName, CommandType.StoredProcedure, paramNames, paramValues, transactionYN);
        }

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(int timeOutInSeconds, string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues, bool transactionYN)
        {
            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    SqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            loadCommand.CommandTimeout = timeOutInSeconds;
            ExecuteNonQuery(transactionYN);
        }


        #endregion

        #region ExecuteNonQuery - Use Trans

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(string storedProcName, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            ExecuteNonQuery(120, storedProcName, paramNames, paramValues, trans);
        }

        /// <summary>
        /// Execute s SQL
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public void ExecuteNonQuery(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    SqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            loadCommand.CommandTimeout = timeOutInSeconds;

            ExecuteNonQuery(trans);
        }


        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteScalar(120, storedProcName, paramNames, paramValues, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteScalar(timeOutInSeconds, storedProcName, paramNames, paramValues, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string storedProcName, string[] paramNames, object[] paramValues, bool transactionYN)
        {
            return ExecuteScalar(120, storedProcName, paramNames, paramValues, transactionYN);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues, bool transactionYN)
        {
            object objReturn = null;
            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    SqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            loadCommand.CommandTimeout = timeOutInSeconds;

            objReturn = ExecuteScalar(transactionYN);
            return objReturn;
        }

        #endregion

        #region ExecuteScalar - Trans

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string storedProcName, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            return ExecuteScalar(120, storedProcName, paramNames, paramValues, trans);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(int timeOutInSeconds, string storedProcName, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            object objReturn = null;
            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    SqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            loadCommand.CommandTimeout = timeOutInSeconds;

            objReturn = ExecuteScalar(trans);
            return objReturn;
        }

        #endregion

        #region ExecuteUpdateData

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public bool ExecuteUpdateData2(ref DataSet changedDS, string storedProcName, string[] paramNames,
            string tableName, bool useTransaction)
        {
            bool boolData = false;
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetSqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetSqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetSqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            boolData = UpdateData(changedDS, tableName, useTransaction);
            return boolData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="realParamName"></param>
        /// <returns></returns>
        private string GetSqlDbTypeName(string paramName, out string realParamName)
        {
            realParamName = paramName;
            string sqlDbTypeName = string.Empty;
            var temp = Regex.Escape(paramName);
            var temp1 = Regex.Matches(temp, "@");
            int count = Regex.Matches(Regex.Escape(paramName), "@").Count;
            if (count == 2)
            {
                realParamName = paramName.Substring(0, paramName.LastIndexOf('@'));
                sqlDbTypeName = paramName.Substring(paramName.LastIndexOf('@') + 1);
            }

            return sqlDbTypeName;
        }

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public bool ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames,
            string tableName, bool useTransaction)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");

            string paramName = string.Empty;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, useTransaction);
        }

        /// <summary>
        ///	
        /// </summary>
        public bool ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames,
            string[] paramTypes, int[] paramSizes, string tableName, bool useTransaction)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, (((string)paramNames[i])).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, useTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramTypeName"></param>
        /// <returns></returns>
        private SqlDbType GetParamTypeByName(string paramTypeName)
        {
            switch (paramTypeName.ToLower())
            {
                case "varchar":
                    return SqlDbType.VarChar;
                case "char":
                    return SqlDbType.Char;
                case "bit":
                    return SqlDbType.Bit;
                case "decimal":
                    return SqlDbType.Decimal;
                case "numeric":
                case "integer":
                    return SqlDbType.Float;
                case "float":
                case "double":
                    return SqlDbType.Float;
                case "bigint":
                    return SqlDbType.BigInt;
                case "binary":
                    return SqlDbType.Binary;
                case "image":
                    return SqlDbType.Image;
                case "int":
                    return SqlDbType.Int;
                case "money":
                    return SqlDbType.Money;
                case "nchar":
                    return SqlDbType.NChar;
                case "ntext":
                    return SqlDbType.NText;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "real":
                    return SqlDbType.Real;
                case "smalldatetime":
                    return SqlDbType.SmallDateTime;
                case "datetime":
                case "date":
                    return SqlDbType.DateTime;
                case "smallint":
                    return SqlDbType.SmallInt;
                case "smallmoney":
                    return SqlDbType.SmallMoney;
                case "text":
                    return SqlDbType.Text;
                case "timestamp":
                    return SqlDbType.Timestamp;
                case "tinyInt":
                    return SqlDbType.TinyInt;
                case "variant": 
                default:
                    return SqlDbType.Variant;
            }
        }

        #endregion

        #region ExecuteUpdateData

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public ResultMessage ExecuteUpdateData2(ref DataSet changedDS, string storedProcName, string[] paramNames,
            string tableName, SqlTransaction trans)
        {
            ResultMessage boolData = new ResultMessage();

            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            boolData = UpdateData(changedDS, tableName, trans);

            return boolData;
        }

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public ResultMessage ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames,
            string tableName, SqlTransaction trans)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");

            string paramName = string.Empty;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, trans);
        }

        /// <summary>
        ///	
        /// </summary>
        public ResultMessage ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames,
            string[] paramTypes, int[] paramSizes, string tableName, SqlTransaction trans)
        {

            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", SqlDbType.Char, 6, "INSERT");
            ParameterDirection paramDirection = ParameterDirection.Input;

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    //AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                    //    ParameterDirection.Input, (((string)paramNames[i])).Substring(1));

                    SqlDbType pType = GetParamTypeByName(paramTypes[i]);
                    
                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", SqlDbType.Char, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    //AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                    //    ParameterDirection.Input, ((string)paramNames[i]).Substring(1));

                    SqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", SqlDbType.Char, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    SqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            return UpdateData(changedDS, tableName, trans);
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Start a new transaction
        /// </summary>
        /// <returns></returns>
        public SqlTransaction BeginTransaction()
        {
            SqlTransaction trans = null;
            int connTimes = CONN_TIMES + 2;

            while (connTimes > 0)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    trans = _connection.BeginTransaction();
                    break;
                }
                catch// (System.Exception ex)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                    connTimes--;
                    System.Threading.Thread.Sleep(INTERVAL_SCHEDULE);
                }
            }

            return trans;
        }

        #endregion
    }
}
