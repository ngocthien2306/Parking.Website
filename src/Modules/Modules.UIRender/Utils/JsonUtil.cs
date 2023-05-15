using InfrastructureCore.Utils;
using Microsoft.IdentityModel.Protocols;
using Modules.UIRender.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Modules.UIRender.Utils
{
    public static class JsonUtil
    {
        public static List<DynamicJsonModel> ReadJsonOfDynamicPage()
        {
            LogWriter log = new LogWriter("");
            try
            {               
                log.LogWrite("LOG ReadJsonOfDynamicPage" + Directory.GetCurrentDirectory());
#if (DEBUG)
                string path = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().LastIndexOf('\\')) + "\\Modules\\Modules.UIRender\\Dynamic.json";
                var json = File.ReadAllText(path);
#elif (RELEASE)
                string path = Directory.GetCurrentDirectory() + "\\Dynamic.json";
                var json = File.ReadAllText(path);
#endif
                var results = JsonConvert.DeserializeObject<List<DynamicJsonModel>>(json);
                return results;
            }
            catch(Exception e)
            {
                log.LogWrite(" Log ex " + e.Message);
                return new List<DynamicJsonModel>();
            }
        }
    }
}
