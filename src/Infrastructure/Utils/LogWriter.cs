using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InfrastructureCore.Utils
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            //m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                //string FolderPath = System.Web.HttpContext.Current.Server.MapPath("~\\Pleiger_Log");
                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pleiger_Log");
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);
                using (StreamWriter w = File.AppendText(FolderPath + "\\log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception Ex)
            {
                LogWrite("Error Log: " + Environment.NewLine + Ex.ToString());
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception)
            {
            }
            finally
            {
                txtWriter.Close();
                txtWriter.Dispose();
            }
        }
    }
}
