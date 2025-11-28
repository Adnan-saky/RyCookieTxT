using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using RyCookieText.Services;
using RyCookieText.Views;

namespace RyCookieText.Services
{
    /// <summary>
    /// Manages the system tray icon and context menu
    /// </summary>
    public class TrayIconManager : IDisposable
    {
        private readonly TaskbarIcon _trayIcon;
        private readonly ExpansionEngine _expansionEngine;
        private readonly SnippetManager _snippetManager;
        private MainWindow? _mainWindow;

        public TrayIconManager(ExpansionEngine expansionEngine, SnippetManager snippetManager)
        {
            _expansionEngine = expansionEngine;
            _snippetManager = snippetManager;

            // Create tray icon
            _trayIcon = new TaskbarIcon
            {
                ToolTipText = "RyCookie Text - Text Expander",
                IconSource = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("pack://application:,,,/Resources/icon.ico"))
            };

            // Create context menu
            var contextMenu = new System.Windows.Controls.ContextMenu();

            // Open Snippet Manager
            var openMenuItem = new System.Windows.Controls.MenuItem
            {
                Header = "📝 Open Snippet Manager"
            };
            openMenuItem.Click += OpenMenuItem_Click;
            contextMenu.Items.Add(openMenuItem);

            contextMenu.Items.Add(new System.Windows.Controls.Separator());

            // Enable/Disable Expansion
            var toggleMenuItem = new System.Windows.Controls.MenuItem
            {
                Header = "✓ Enable Expansion",
                IsCheckable = true,
                IsChecked = _expansionEngine.IsEnabled
            };
            toggleMenuItem.Click += ToggleMenuItem_Click;
            contextMenu.Items.Add(toggleMenuItem);

            contextMenu.Items.Add(new System.Windows.Controls.Separator());

            // Exit
            var exitMenuItem = new System.Windows.Controls.MenuItem
            {
                Header = "❌ Exit"
            };
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);

            _trayIcon.ContextMenu = contextMenu;

            // Double-click to open
            _trayIcon.TrayLeftMouseDown += (s, e) => ShowMainWindow();

            // Subscribe to expansion events
            _expansionEngine.SnippetExpanded += OnSnippetExpanded;
        }

        private void OpenMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void ToggleMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem menuItem)
            {
                _expansionEngine.IsEnabled = menuItem.IsChecked;
                menuItem.Header = menuItem.IsChecked ? "✓ Enable Expansion" : "Enable Expansion";

                // Update settings
                var settings = (Models.AppSettings)Application.Current.Properties["Settings"]!;
                settings.IsExpansionEnabled = _expansionEngine.IsEnabled;

                // Show notification
                _trayIcon.ShowBalloonTip(
                    "RyCookie Text",
                    $"Text expansion {(menuItem.IsChecked ? "enabled" : "disabled")}",
                    BalloonIcon.Info);
            }
        }

        private void ExitMenuItem_Click(object? sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ShowMainWindow()
        {
            if (_mainWindow == null || !_mainWindow.IsLoaded)
            {
                _mainWindow = new MainWindow();
            }

            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }

        private void OnSnippetExpanded(object? sender, SnippetExpandedEventArgs e)
        {
            // Optional: Show notification when snippet is expanded
            // Uncomment if you want notifications
            // _trayIcon.ShowBalloonTip(
            //     "Snippet Expanded",
            //     $"{e.Snippet.Keyword} → {e.Snippet.Content.Substring(0, Math.Min(50, e.Snippet.Content.Length))}",
            //     BalloonIcon.Info);
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }
    }
}
