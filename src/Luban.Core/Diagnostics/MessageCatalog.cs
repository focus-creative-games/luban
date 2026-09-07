// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Globalization;
using System.Reflection;
using System.Text.Json;
using NLog;

namespace Luban.Diagnostics;

public static class MessageCatalog
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private static readonly Dictionary<string, Dictionary<string, string>> s_catalogs = new(StringComparer.Ordinal);
    private static Dictionary<string, string> s_current = new(StringComparer.Ordinal);
    private static string s_locale = "en";
    private static string s_unknownLocale;
    private static bool s_loaded;

    public static string Locale => s_locale;

    public static void Init(string locale)
    {
        EnsureLoaded();
        s_locale = ResolveLocale(locale, out s_unknownLocale);
        s_current = s_catalogs[s_locale];
        ValidateKeyParity();
        if (s_unknownLocale != null)
        {
            s_logger.Warn(Format("warn.cli.unknown_locale", s_unknownLocale, s_locale));
        }
    }

    public static string Format(string key, params object[] args)
    {
        EnsureLoaded();
        if (!s_current.TryGetValue(key, out var template))
        {
            if (args == null || args.Length == 0)
            {
                return key;
            }
            return $"{key}: {string.Join(", ", args)}";
        }

        if (args == null || args.Length == 0)
        {
            return template;
        }

        try
        {
            return string.Format(CultureInfo.InvariantCulture, template, args);
        }
        catch (FormatException)
        {
            return $"{key}: {string.Join(", ", args)}";
        }
    }

    private static void EnsureLoaded()
    {
        if (s_loaded)
        {
            return;
        }

        lock (s_catalogs)
        {
            if (s_loaded)
            {
                return;
            }

            s_catalogs["en"] = LoadResource("en");
            s_catalogs["zh"] = LoadResource("zh");
            s_locale = ResolveLocale(null, out _);
            s_current = s_catalogs[s_locale];
            s_loaded = true;
        }
    }

    private static string ResolveLocale(string locale, out string unknownLocale)
    {
        unknownLocale = null;
        if (string.IsNullOrWhiteSpace(locale))
        {
            string ui = CultureInfo.CurrentUICulture.Name;
            return ui.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ? "zh" : "en";
        }

        string normalized = locale.Trim().ToLowerInvariant();
        if (normalized == "zh" || normalized.StartsWith("zh-"))
        {
            return "zh";
        }
        if (normalized == "en" || normalized.StartsWith("en-"))
        {
            return "en";
        }

        unknownLocale = locale;
        return "en";
    }

    private static Dictionary<string, string> LoadResource(string locale)
    {
        var assembly = typeof(MessageCatalog).Assembly;
        string resourceName = $"Luban.Resources.messages_{locale}.json";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"missing embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        string json = reader.ReadToEnd();
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        return dict != null
            ? new Dictionary<string, string>(dict, StringComparer.Ordinal)
            : new Dictionary<string, string>(StringComparer.Ordinal);
    }

    private static void ValidateKeyParity()
    {
        if (!s_catalogs.TryGetValue("en", out var en) || !s_catalogs.TryGetValue("zh", out var zh))
        {
            return;
        }

        foreach (var key in en.Keys.Except(zh.Keys))
        {
            s_logger.Warn("message key '{0}' exists in en but missing in zh", key);
        }
        foreach (var key in zh.Keys.Except(en.Keys))
        {
            s_logger.Warn("message key '{0}' exists in zh but missing in en", key);
        }
    }
}
