namespace Luban.Common.Utils
{
    public static class LogUtil
    {
        public static void InitSimpleNLogConfigure(NLog.LogLevel minConsoleLogLevel)
        {
            var logConfig = new NLog.Config.LoggingConfiguration();
            NLog.Layouts.Layout layout;
            if (minConsoleLogLevel <= NLog.LogLevel.Debug)
            {
                layout = NLog.Layouts.Layout.FromString("${longdate}|${level:uppercase=true}|${callsite}:${callsite-linenumber}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
            }
            else
            {
                layout = NLog.Layouts.Layout.FromString("${longdate}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
            }
            logConfig.AddTarget("console", new NLog.Targets.ColoredConsoleTarget() { Layout = layout });
            logConfig.AddRule(minConsoleLogLevel, NLog.LogLevel.Off, new NLog.Targets.NullTarget(), "Bright.Net.Channels.*", true);
            logConfig.AddRule(minConsoleLogLevel, NLog.LogLevel.Fatal, "console");
            NLog.LogManager.Configuration = logConfig;
        }
    }
}
