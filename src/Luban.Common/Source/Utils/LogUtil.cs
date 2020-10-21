namespace Luban.Common.Utils
{
    public static class LogUtil
    {
        public static void InitSimpleNLogConfigure(NLog.LogLevel minConsoleLogLevel)
        {
            var logConfig = new NLog.Config.LoggingConfiguration();
            //var layout = NLog.Layouts.Layout.FromString("${longdate}|${level:uppercase=true}|${threadid}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
            var layout = NLog.Layouts.Layout.FromString("${longdate}|${message}${onexception:${newline}${exception:format=tostring}${exception:format=StackTrace}}");
            logConfig.AddTarget("console", new NLog.Targets.ColoredConsoleTarget() { Layout = layout });
            logConfig.AddRule(minConsoleLogLevel, NLog.LogLevel.Fatal, "console");
            NLog.LogManager.Configuration = logConfig;
        }
    }
}
