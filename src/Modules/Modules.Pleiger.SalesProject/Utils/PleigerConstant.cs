namespace Modules.Pleiger.Utils
{
    public static class PleigerConstant
    {
        /// <summary>
        /// Replace special symbol when search data
        /// Ex: itemName : V1000[20]
        /// </summary>
        public static string REGEX_REPLACE_DATA_SEARCH = @"[-/[%^*()!+\]]";
    }
}
