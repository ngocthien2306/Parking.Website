using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace LazZiya.ExpressLocalization
{
    /// <summary>
    /// Reads resource key and return relevant localized string value
    /// </summary>
    internal class GenericResourceReader
    {
        /// <summary>
        /// Reads resource key and return relevant localized string value
        /// </summary>
        /// <typeparam name="T">type of resource file that containes localized string values</typeparam>
        /// <param name="culture">Culture name e.g. ar-SY</param>
        /// <param name="code">key name to look for</param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static string GetValue<T>(string culture, string code, params object[] args) where T : class
        {
            return GetValue(typeof(T), culture, code, args).Value;
        }

        /// <summary>
        /// Reads resource key and return relevant localized string value
        /// </summary>
        /// <param name="resourceSource">Type of the resource that contains the localized strings</param>
        /// <param name="culture">Culture name e.g. ar-SY</param>
        /// <param name="code">key name to look for</param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static LocalizedHtmlString GetValue(Type resourceSource, string culture, string code, params object[] args)
        {
            //var _res = new System.Resources.ResourceManager(resourceSource);

            var cultureInfo = string.IsNullOrWhiteSpace(culture)
                ? CultureInfo.CurrentCulture
                : CultureInfo.GetCultureInfo(culture);

            bool _resourceNotFound;
            string _value;

            try
            {
                //_value = _res.GetString(code, cultureInfo);

                string shortCulture = culture.Contains("-") ? culture.Substring(0, culture.IndexOf("-")) : culture;
                _value = FileResourceReader.Instance.GetResourceString(shortCulture, code);
                _resourceNotFound = false;
            }
            catch (MissingSatelliteAssemblyException)
            {
                _resourceNotFound = true;
                _value = code;
            }
            catch (MissingManifestResourceException)
            {
                _resourceNotFound = true;
                _value = code;
            }

            return args == null
                ? new LocalizedHtmlString(code, _value, _resourceNotFound)
                : new LocalizedHtmlString(code, _value, _resourceNotFound, args);
        }
    }

    
}
