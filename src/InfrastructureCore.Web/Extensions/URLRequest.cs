using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Web.Extensions
{
    public static class URLRequest
    {
        public static string URLSubstring(string urlPath)
        {
            var count = urlPath.Split("/")[1].Length + 1;
            var curUrlTemp = urlPath.Substring(count, urlPath.Length - count);
            return curUrlTemp;
        }
    }
}
