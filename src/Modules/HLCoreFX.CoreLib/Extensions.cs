using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using HLCoreFX.CoreLib.Model;
using Newtonsoft.Json.Linq;

namespace HLCoreFX.CoreLib
{
    public static class Extensions
    {
        static public string[] ToArray(this IEnumerable<string> enumerable, string prefix)
        {
            List<string> spParams = new List<string>();
            enumerable.ToList<string>().ForEach(p => spParams.Add(prefix + p));
            return spParams.ToArray();
        }

        static public object GetProp(this object mapData, string propName)
        {
            var prop = mapData.GetType().GetProperty(propName);
            return prop.GetValue(mapData);
        }

        static public Dictionary<string, object> ToKeyValueDic(this JObject formData)
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            var props = formData.GetType().GetProperties();
            foreach (PropertyInfo pi in props)
            {
                keyValues.Add(pi.Name, pi.GetValue(formData));
            }

            return keyValues;
        }
    }
}
