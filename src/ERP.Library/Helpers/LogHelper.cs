using Serilog;

namespace ERP.Library.Helpers
{
    public static class LogHelper
    {
        private static readonly string BaseLogPath =
            Path.Combine(AppContext.BaseDirectory, "logs");

        private static ILogger CreateFileLogger(params string[] subFolders)
        {
            var logDir = Path.Combine(
                new[] { BaseLogPath }.Concat(subFolders).ToArray()
            );

            Directory.CreateDirectory(logDir);

            var logFile = Path.Combine(logDir, "log-.txt");

            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
        }

        /// <summary>Error Logger</summary>
        public static readonly ILogger ErrorLogger =
            CreateFileLogger("Error");
    }
}
