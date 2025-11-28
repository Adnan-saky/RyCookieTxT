using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RyCookieText.Models;
using RyCookieText.Services;

namespace RyCookieText.Views
{
    public partial class MainWindow : Window
    {
        private readonly SnippetManager _snippetManager;
        private readonly ExpansionEngine _expansionEngine;
        private readonly ThemeManager _themeManager;
        private readonly CsvImportService _csvImportService;

        public MainWindow()
        {
            InitializeComponent();

            // Get services from application
            _snippetManager = (SnippetManager)Application.Current.Properties["SnippetManager"]!;
            _expansionEngine = (ExpansionEngine)Application.Current.Properties["ExpansionEngine"]!;
            _themeManager = (ThemeManager)Application.Current.Properties["ThemeManager"]!;
            _csvImportService = new CsvImportService();

            // Set initial state
            ExpansionToggle.IsChecked = _expansionEngine.IsEnabled;
            UpdateThemeButton();

            // Load snippets
            LoadSnippets();

            // Subscribe to changes
            _snippetManager.SnippetsChanged += (s, e) => LoadSnippets();
        }

        private void LoadSnippets(string? searchQuery = null)
        {
            var snippets = string.IsNullOrWhiteSpace(searchQuery)
                ? _snippetManager.GetAll()
                : _snippetManager.Search(searchQuery);

            SnippetsGrid.ItemsSource = snippets.OrderBy(s => s.Keyword).ToList();
            StatusText.Text = $"{snippets.Count} snippet(s)";
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadSnippets(SearchBox.Text);
        }

        private void SnippetsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool hasSelection = SnippetsGrid.SelectedItem != null;
            EditButton.IsEnabled = hasSelection;
            DeleteButton.IsEnabled = hasSelection;
        }

        private void SnippetsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SnippetsGrid.SelectedItem != null)
            {
                EditButton_Click(sender, e);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SnippetEditDialog(null);
            if (dialog.ShowDialog() == true && dialog.EditedSnippet != null)
            {
                if (_snippetManager.Add(dialog.EditedSnippet))
                {
                    StatusText.Text = "Snippet added successfully";
                }
                else
                {
                    MessageBox.Show("Failed to add snippet. The keyword may already exist.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetsGrid.SelectedItem is Snippet selectedSnippet)
            {
                var dialog = new SnippetEditDialog(selectedSnippet);
                if (dialog.ShowDialog() == true && dialog.EditedSnippet != null)
                {
                    if (_snippetManager.Update(dialog.EditedSnippet))
                    {
                        StatusText.Text = "Snippet updated successfully";
                    }
                    else
                    {
                        MessageBox.Show("Failed to update snippet. The keyword may already exist.", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetsGrid.SelectedItem is Snippet selectedSnippet)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the snippet '{selectedSnippet.Keyword}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_snippetManager.Delete(selectedSnippet.Id))
                    {
                        StatusText.Text = "Snippet deleted successfully";
                    }
                }
            }
        }

        private void ExpansionToggle_Changed(object sender, RoutedEventArgs e)
        {
            // Skip if called during initialization before services are loaded
            if (_expansionEngine == null) return;
            
            _expansionEngine.IsEnabled = ExpansionToggle.IsChecked ?? false;
            
            var settings = (AppSettings)Application.Current.Properties["Settings"]!;
            settings.IsExpansionEnabled = _expansionEngine.IsEnabled;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Import Snippets from CSV"
            };

            if (dialog.ShowDialog() == true)
            {
                var result = _csvImportService.ImportFromCsv(dialog.FileName);

                // Add imported snippets
                int added = 0;
                foreach (var snippet in result.ImportedSnippets)
                {
                    if (_snippetManager.Add(snippet))
                    {
                        added++;
                    }
                }

                // Show result
                var message = $"Import completed:\n" +
                             $"✅ Successfully imported: {added}\n" +
                             $"⚠️ Skipped: {result.SkippedCount}\n" +
                             $"❌ Errors: {result.ErrorCount}";

                if (result.Errors.Count > 0)
                {
                    message += "\n\nErrors:\n" + string.Join("\n", result.Errors.Take(5));
                    if (result.Errors.Count > 5)
                    {
                        message += $"\n... and {result.Errors.Count - 5} more";
                    }
                }

                MessageBox.Show(message, "Import Results", MessageBoxButton.OK, 
                    result.ErrorCount > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);

                StatusText.Text = $"Imported {added} snippet(s)";
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Export Snippets to CSV",
                FileName = "snippets.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var snippets = _snippetManager.GetAll();
                if (_csvImportService.ExportToCsv(dialog.FileName, snippets))
                {
                    MessageBox.Show($"Successfully exported {snippets.Count} snippet(s) to CSV.",
                        "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    StatusText.Text = $"Exported {snippets.Count} snippet(s)";
                }
                else
                {
                    MessageBox.Show("Failed to export snippets to CSV.",
                        "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _themeManager.ToggleTheme();
            UpdateThemeButton();

            // Save theme preference
            var settings = (AppSettings)Application.Current.Properties["Settings"]!;
            settings.CurrentTheme = _themeManager.CurrentTheme;

            StatusText.Text = $"Switched to {_themeManager.CurrentTheme} theme";
        }

        private void UpdateThemeButton()
        {
            ThemeToggleButton.Content = _themeManager.CurrentTheme == AppTheme.Dark 
                ? "☀️ Light" 
                : "🌙 Dark";
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Hide instead of close
            e.Cancel = true;
            Hide();
        }
    }
}
