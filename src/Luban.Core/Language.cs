using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace Luban
{
    public class Language
    {
        public const string English = "English";
        public const string Chinese = "Chinese";

        static ResourceManager? s_ResourceManager;

        public static void Init()
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;
            var languageFileName = string.Empty;
            var culture = default(CultureInfo);

            if (language == "zh-CN")
            {
                culture = new CultureInfo("zh-CN");
                languageFileName = Chinese;
            }
            else
            {
                culture = new CultureInfo("en-US");
                languageFileName = English;
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            s_ResourceManager = new ResourceManager($"Luban.Assets.Language_{languageFileName}", typeof(Language).Assembly);
        }

        public static string GetString(string key, params string[] args)
        {
            return string.Format(s_ResourceManager?.GetString(key, Thread.CurrentThread.CurrentCulture) ?? string.Empty, args);
        }
    }
}
