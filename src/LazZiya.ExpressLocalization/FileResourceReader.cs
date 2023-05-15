using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LazZiya.ExpressLocalization
{
    public class FileResourceReader
    {
        private const string RES_PATH = "resources";
        private const string RES_FILE_PREFIX = "resources_";

        Dictionary<string, Dictionary<string, string>> resxValues = null;

        /// <summary>
        /// File & version
        /// </summary>
        Dictionary<string, string> resxVersions = null;

        static FileResourceReader m_instance = null;

        /// <summary>
        /// 
        /// </summary>
        public static FileResourceReader Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new FileResourceReader();
                }

                return m_instance;
            }
        }

        internal string ResourcesFolder
        {
            get
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(path, RES_PATH);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileResourceReader()
        {
            resxVersions = new Dictionary<string, string>();
            resxValues = new Dictionary<string, Dictionary<string, string>>();
            var files = Directory.GetFiles(ResourcesFolder, RES_FILE_PREFIX + "*.txt");
            foreach (var filePath in files)
            {
                FileInfo fi = new FileInfo(filePath);
                var culture = Path.GetFileNameWithoutExtension(filePath).Substring(RES_FILE_PREFIX.Length);
                resxVersions.Add(culture, fi.LastWriteTime.ToString("yyMMddHHmmss"));
                resxValues.Add(culture, new Dictionary<string, string>());

                CompareAndReload(culture, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        void CompareAndReload(string culture, bool forceLoad)
        {
            var resxFile = Path.Combine(ResourcesFolder, RES_FILE_PREFIX + culture + ".txt");
            if (!File.Exists(resxFile))
            {
                return;
            }

            var fi = new FileInfo(resxFile);
            string currVersion = fi.LastWriteTime.ToString("yyMMddHHmmss");
            if (forceLoad || (resxVersions.ContainsKey(culture) && currVersion != resxVersions[culture]))
            {
                // reload resources
                LoadResources(culture);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        internal void LoadResources(string culture)
        {
            var files = Directory.GetFiles(ResourcesFolder, RES_FILE_PREFIX + (string.IsNullOrEmpty(culture) ? "*" : culture) + ".txt");
            if (files.Length == 0)
            {
                return;
            }

            var texts = File.ReadAllLines(files[0], Encoding.UTF8);
            foreach (var item in texts)
            {
                var values = item.Split("<=>");
                if (values.Length > 0)
                {
                    string key = WelformValue(values[0]);
                    if (resxValues[culture].ContainsKey(key))
                    {
                        resxValues[culture][key] = values.Length > 1 ? values[1] : values[0];
                    }
                    else
                    {
                        resxValues[culture].Add(key, values.Length > 1 ? values[1] : values[0]);
                    }
                }
            }

            var fi = new FileInfo(files[0]);
            resxVersions[culture] = fi.LastWriteTime.ToString("yyMMddHHmmss");
        }

        string WelformValue(string value)
        {
            if (value.Contains("\""))
            {
                value = value.Replace("\"", "'");
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetResourceString(string culture, string code)
        {
            CompareAndReload(culture, false);

            if (!resxValues.ContainsKey(culture))
            {
                return code;
            }

            string fcode = WelformValue(code);

            if (!resxValues[culture].ContainsKey(fcode))
            {
                return code;
            }

            return resxValues[culture][fcode];
        }
    }
}
