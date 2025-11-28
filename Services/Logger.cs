using System;
using System.IO;

namespace RyCookieText.Services
{
    public static class Logger
    {
        private static string LogPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log");

        public static void Log(string message)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}";
                File.AppendAllText(LogPath, logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        public static void Log(Exception ex, string context = "")
        {
            Log($"ERROR {context}: {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Log($"INNER ERROR: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.StackTrace}");
            }
        }
    }
}
