using System;
using System.Windows;

namespace RyCookieText.Services
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Manages application theme switching
    /// </summary>
    public class ThemeManager
    {
        private const string LightThemeUri = "Themes/LightTheme.xaml";
        private const string DarkThemeUri = "Themes/DarkTheme.xaml";

        public AppTheme CurrentTheme { get; private set; }

        public ThemeManager()
        {
            CurrentTheme = AppTheme.Dark; // Default theme
        }

        /// <summary>
        /// Apply a theme to the application
        /// </summary>
        public void ApplyTheme(AppTheme theme)
        {
            CurrentTheme = theme;

            var themeUri = theme == AppTheme.Light ? LightThemeUri : DarkThemeUri;

            // Clear existing theme dictionaries
            Application.Current.Resources.MergedDictionaries.Clear();

            // Load new theme
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(themeUri, UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        /// <summary>
        /// Toggle between light and dark themes
        /// </summary>
        public void ToggleTheme()
        {
            var newTheme = CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            ApplyTheme(newTheme);
        }
    }
}
