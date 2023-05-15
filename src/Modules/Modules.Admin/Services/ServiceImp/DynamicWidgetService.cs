using InfrastructureCore.DAL;
using Modules.Admin.Models.WidgetModels;
using Modules.Admin.Services.IService;
using Modules.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Services.ServiceImp
{
    public class DynamicWidgetService : IDynamicWidgetService
    {
        public dynamic ExecuteProcedure(string procName, ICollection<SYWidgetStoreProcedureParams> paras, string connectionType)//G013C001
        {
            var rtnList = new List<object>();
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(connectionType))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(connectionType)))
                    {
                        List<string> arr = new List<string>();
                        List<object> arrValue = new List<object>();
                        if (paras != null)
                        {
                            foreach (var item in paras)
                            {
                                // Note: MSQL, SQL having @
                                // Oracle not have it
                                arr.Add("@" + item.ParamName);
                                arrValue.Add(item.ParamValue);
                            }
                        }
                        var result = conn.ExecuteQuery2(procName, arr.ToArray(), arrValue.ToArray());
                        //List<Field> fields = new List<Field>();
                        //foreach (var item in result)
                        //{
                        //    foreach (var item1 in item)
                        //    {
                        //        fields.Add(new Field { FieldName = item1.Key, FieldType = item1.Type });
                        //    }
                        
                            //}
                            //var number = result.FirstOrDefault();
                            //List<Field> fields_1 = new List<Field>();
                            //if (result.Count > 0)
                            //{
                            //    for (int i = 0; i < number.Count; i++)
                            //    {
                            //        fields_1.Add(new Field { FieldName = number[i].Key, FieldType = number[i].Type });
                            //    }
                            //}


                            //var fieldNames = fields.Select(x => x.FieldName);
                            ///// var _class = MyTypeBuilder.CompileResultType(fields);
                            //var _class = MyTypeBuilder.CompileResultType(fields_1);
                            //var propts = _class.GetProperties();
                            //object val;
                            ////var model = Activator.CreateInstance(_class);
                            //foreach (var item in result)
                            //{
                            //    var model = Activator.CreateInstance(_class);
                            //    foreach (var l in propts)
                            //    {
                            //        var temp = item.Where(m => m.Key == l.Name).FirstOrDefault();
                            //        if (temp != null)
                            //        {
                            //            l.SetValue(model, temp.Value);
                            //        }
                            //        else
                            //        {
                            //            l.SetValue(model, null);
                            //        }

                            //    }
                            //    rtnList.Add(model);
                            //}
                        }
                }
                return rtnList;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
