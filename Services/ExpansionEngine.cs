using System;
using System.Threading.Tasks;
using RyCookieText.Models;

namespace RyCookieText.Services
{
    /// <summary>
    /// Orchestrates all services to provide text expansion functionality
    /// </summary>
    public class ExpansionEngine : IDisposable
    {
        private readonly KeyboardHook _keyboardHook;
        private readonly BufferManager _bufferManager;
        private readonly SnippetMatcher _snippetMatcher;
        private readonly SnippetReplacer _snippetReplacer;
        private readonly SnippetManager _snippetManager;
        
        private bool _isEnabled;
        private bool _isProcessingReplacement;

        public event EventHandler<SnippetExpandedEventArgs>? SnippetExpanded;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                if (!_isEnabled)
                {
                    _bufferManager.Clear();
                }
            }
        }

        public ExpansionEngine(SnippetManager snippetManager, AppSettings settings)
        {
            _snippetManager = snippetManager;
            _keyboardHook = new KeyboardHook();
            _bufferManager = new BufferManager(settings.BufferSize);
            _snippetMatcher = new SnippetMatcher(snippetManager);
            _snippetReplacer = new SnippetReplacer(settings.KeystrokeDelayMs);
            
            _isEnabled = settings.IsExpansionEnabled;
            _isProcessingReplacement = false;

            _keyboardHook.KeyPressed += OnKeyPressed;
        }

        /// <summary>
        /// Starts the expansion engine
        /// </summary>
        public void Start()
        {
            _keyboardHook.Install();
        }

        /// <summary>
        /// Stops the expansion engine
        /// </summary>
        public void Stop()
        {
            _keyboardHook.Uninstall();
        }

        private async void OnKeyPressed(object? sender, KeyboardEventArgs e)
        {
            // Skip if disabled or currently processing a replacement
            if (!_isEnabled || _isProcessingReplacement)
                return;

            try
            {
                // Update buffer
                _bufferManager.ProcessKeyEvent(e);

                // Get current buffer and last character
                var buffer = _bufferManager.GetBuffer();
                if (string.IsNullOrEmpty(buffer))
                    return;

                char lastChar = buffer[buffer.Length - 1];

                // Try to find a match
                var match = _snippetMatcher.FindMatch(buffer, lastChar);
                
                if (match != null)
                {
                    // Set flag to prevent recursive processing
                    _isProcessingReplacement = true;

                    // Small delay to ensure the trigger character is processed
                    await Task.Delay(50);

                    // Perform replacement
                    await _snippetReplacer.ReplaceAsync(match);

                    // Clear buffer after replacement
                    _bufferManager.Clear();

                    // Raise event
                    SnippetExpanded?.Invoke(this, new SnippetExpandedEventArgs(match.Snippet));

                    // Reset flag
                    _isProcessingReplacement = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in expansion engine: {ex.Message}");
                _isProcessingReplacement = false;
            }
        }

        public void Dispose()
        {
            Stop();
            _keyboardHook.Dispose();
        }
    }

    /// <summary>
    /// Event args for snippet expansion
    /// </summary>
    public class SnippetExpandedEventArgs : EventArgs
    {
        public Snippet Snippet { get; }

        public SnippetExpandedEventArgs(Snippet snippet)
        {
            Snippet = snippet;
        }
    }
}
