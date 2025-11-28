using System;
using RyCookieText.Services;

namespace RyCookieText.Models
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class AppSettings
    {
        public bool IsExpansionEnabled { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        public string SnippetsFilePath { get; set; } = "snippets.json";
        public int BufferSize { get; set; } = 50;
        public int KeystrokeDelayMs { get; set; } = 5;
        public AppTheme CurrentTheme { get; set; } = AppTheme.Dark;

        /// <summary>
        /// Creates default settings
        /// </summary>
        public static AppSettings CreateDefault()
        {
            return new AppSettings();
        }
    }
}
