using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models.WidgetModels
{
    public class SYWidgetStoreProcedureParams
    {
        public int No { get; set; }
        public int Id { get; set; }
        public string StoreProcedureName { get; set; }
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
        public DataType DataType { get; set; }
        public ParamType ParamType { get; set; }
    }
    public enum DataType
    {
        Varchar,
        Int,
        Datetime
    }
    public enum ParamType
    {
       Input,Output
    }

}
