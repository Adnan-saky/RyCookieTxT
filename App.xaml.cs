using System;
using System.IO;
using System.Windows;
using RyCookieText.Models;
using RyCookieText.Services;

namespace RyCookieText
{
    public partial class App : Application
    {
        private SnippetManager? _snippetManager;
        private ExpansionEngine? _expansionEngine;
        private TrayIconManager? _trayIconManager;
        private ThemeManager? _themeManager;
        private AppSettings? _settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try 
            {
                Logger.Log("Application_Startup: Started");
                
                // Show splash screen
                Logger.Log("Application_Startup: Creating SplashScreen...");
                var splash = new Views.SplashScreen();
                splash.Show();
                Logger.Log("Application_Startup: SplashScreen shown");

                // Initialize in background
                System.Threading.Tasks.Task.Run(async () =>
                {
                    Logger.Log("Startup: Background task started");
                    await System.Threading.Tasks.Task.Delay(500); // Let splash show

                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            Logger.Log("Startup: Loading settings...");
                            // Load settings
                            _settings = LoadSettings();
                            if (_settings == null)
                            {
                                Logger.Log("Startup: Settings are null, creating default");
                                _settings = AppSettings.CreateDefault();
                            }
                            Logger.Log($"Startup: Settings loaded. Theme={_settings.CurrentTheme}, Expansion={_settings.IsExpansionEnabled}");

                        // Initialize theme manager and apply saved theme
                        Logger.Log("Startup: Initializing ThemeManager...");
                        _themeManager = new ThemeManager();
                        _themeManager.ApplyTheme(_settings.CurrentTheme);
                        Logger.Log("Startup: ThemeManager initialized");

                        // Initialize services
                        Logger.Log("Startup: Initializing SnippetManager...");
                        var snippetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settings.SnippetsFilePath);
                        _snippetManager = new SnippetManager(snippetsPath);
                        Logger.Log($"Startup: SnippetManager initialized. Path={snippetsPath}");

                        Logger.Log("Startup: Initializing ExpansionEngine...");
                        _expansionEngine = new ExpansionEngine(_snippetManager, _settings);
                        Logger.Log("Startup: ExpansionEngine initialized");

                        // Start expansion engine
                        Logger.Log("Startup: Starting ExpansionEngine...");
                        _expansionEngine.Start();
                        Logger.Log("Startup: ExpansionEngine started");

                        // Store in application properties for access from windows
                        Logger.Log("Startup: Storing properties...");
                        Current.Properties["SnippetManager"] = _snippetManager;
                        Current.Properties["ExpansionEngine"] = _expansionEngine;
                        Current.Properties["ThemeManager"] = _themeManager;
                        Current.Properties["Settings"] = _settings;
                        Logger.Log("Startup: Properties stored");

                        // Initialize tray icon
                        Logger.Log("Startup: Initializing TrayIconManager...");
                        _trayIconManager = new TrayIconManager(_expansionEngine, _snippetManager);
                        Logger.Log("Startup: TrayIconManager initialized");
                        
                        Logger.Log("Startup: Showing Main Window...");
                        _trayIconManager.ShowMainWindow();
                        Logger.Log("Startup: Main Window shown");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex, "Startup Failure");
                        splash.Close();
                        MessageBox.Show($"Failed to start application: {ex.Message}\n\nSee debug.log for details.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown();
                    }
                });
            });
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Immediate Startup Failure");
                MessageBox.Show($"Critical startup error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Save settings
            if (_settings != null)
            {
                SaveSettings(_settings);
            }

            // Cleanup
            _expansionEngine?.Dispose();
            _trayIconManager?.Dispose();
        }

        private AppSettings LoadSettings()
        {
            try
            {
                var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json) 
                           ?? AppSettings.CreateDefault();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return AppSettings.CreateDefault();
        }

        private void SaveSettings(AppSettings settings)
        {
            try
            {
                var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}
