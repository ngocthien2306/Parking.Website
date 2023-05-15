#region HANBAL Framework Copyright Notice
///
/// Statements should be mentioned here about:
/// - What is this framework about
/// - How is this made?
/// - Open source policy
/// - ...
/// 
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using System.Data;
using System.Data.SqlClient;

using System.Web;
using Microsoft.AspNetCore.Http;

//
// 
// Description of this namespace
//
//
namespace HLCoreFX.CoreLib
{
	
	/// <summary>
    /// HANBAL ExceptionHandler
	/// </summary>
	public class ExceptionHandler
	{
        /// <summary>
        /// Log exception to media and raise exception if raiseException = true
        /// </summary>
        /// <param name="innerException"></param>
        /// <param name="raiseException"></param>
        public static void Publish(System.Exception innerException, bool raiseException)
        {
            // write to all trace listeners
            Trace.WriteLineIf(true, Format(innerException));
            if (raiseException)
            {
                throw innerException;
            }
        }

        /// <summary>
        /// Log exception message to media
        /// </summary>
        /// <param name="traceMessage"></param>
        public static void Publish(string traceMessage)
        {
            // write to all trace listeners
            Trace.WriteLineIf(true, traceMessage);
        }

        /// <summary>
        /// Format exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        static public string Format(System.Exception exception)
        {
            string tmp = string.Empty;
            StringBuilder msg = null;
            DateTime dateTime = DateTime.Now;
            try
            {
                msg = new StringBuilder();
                msg.AppendLine();
                msg.AppendLine("==============[System Error Tracing]==============");
                msg.AppendLine("[CallStackTrace]");
                msg.AppendLine(exception.StackTrace);

                //msg에 현재 트래이싱된 시간을 담도록 한다.
                msg.AppendLine("\r\n[DateTime] : " + dateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                //만약 Sql관련 Exception이면 추가항목을 넣어주도록 한다.
                if (exception.GetType() == typeof(System.Data.SqlClient.SqlException))
                {
                    SqlException sqlErr = (SqlException)exception;
                    msg.Append("\r\n[SqlException] ");
                    msg.Append("\r\nException Type: ").Append(sqlErr.GetType());
                    msg.Append("\r\nErrors: ").Append(sqlErr.ErrorCode);
                    msg.Append("\r\nMessage: ").Append("{" + sqlErr.Message + "}");
                }
                //Sql관련Exception외에 작업들..
                else
                {
                    msg.Append("\r\n[Exception] ");
                    msg.Append("\r\n" + "DetailMsg: {" + exception.Message + "}");
                }
            }
            catch
            {
                //throw ex;
            }

            return msg.ToString();
        }
    }

}
