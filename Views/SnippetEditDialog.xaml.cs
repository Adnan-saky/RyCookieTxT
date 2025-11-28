using System.Windows;
using RyCookieText.Models;

namespace RyCookieText.Views
{
    public partial class SnippetEditDialog : Window
    {
        public Snippet? EditedSnippet { get; private set; }

        public SnippetEditDialog(Snippet? snippet)
        {
            InitializeComponent();

            if (snippet != null)
            {
                // Edit mode
                Title = "Edit Snippet";
                KeywordBox.Text = snippet.Keyword;
                ContentBox.Text = snippet.Content;
                GroupBox.Text = snippet.Group;
                EnabledCheckBox.IsChecked = snippet.IsEnabled;
                EditedSnippet = snippet.Clone();
            }
            else
            {
                // Add mode
                Title = "Add Snippet";
                GroupBox.Text = "Common";
                EditedSnippet = new Snippet();
            }

            KeywordBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(KeywordBox.Text))
            {
                ShowValidationError("Keyword is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(ContentBox.Text))
            {
                ShowValidationError("Content is required");
                return;
            }

            // Update snippet
            if (EditedSnippet != null)
            {
                EditedSnippet.Keyword = KeywordBox.Text.Trim();
                EditedSnippet.Content = ContentBox.Text;
                EditedSnippet.Group = string.IsNullOrWhiteSpace(GroupBox.Text) ? "Default" : GroupBox.Text.Trim();
                EditedSnippet.IsEnabled = EnabledCheckBox.IsChecked ?? true;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowValidationError(string message)
        {
            ValidationMessage.Text = message;
            ValidationMessage.Visibility = Visibility.Visible;
        }
    }
}
