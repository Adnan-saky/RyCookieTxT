using System;
using System.Collections.Generic;
using System.Linq;
using RyCookieText.Models;

namespace RyCookieText.Services
{
    /// <summary>
    /// Matches typed text against snippet keywords
    /// </summary>
    public class SnippetMatcher
    {
        private readonly SnippetManager _snippetManager;
        private readonly char[] _triggerChars = { ' ', '\n', '\t', '.', ',', ';', '!', '?' };

        public SnippetMatcher(SnippetManager snippetManager)
        {
            _snippetManager = snippetManager;
        }

        /// <summary>
        /// Attempts to find a matching snippet from the buffer
        /// </summary>
        /// <param name="buffer">Current typed text buffer</param>
        /// <param name="lastChar">The last character typed</param>
        /// <returns>Matched snippet and keyword length, or null if no match</returns>
        public MatchResult? FindMatch(string buffer, char lastChar)
        {
            // Only check for matches when a trigger character is typed
            if (!_triggerChars.Contains(lastChar))
                return null;

            if (string.IsNullOrEmpty(buffer))
                return null;

            // Get the text before the trigger character
            var textBeforeTrigger = buffer.Length > 1 
                ? buffer.Substring(0, buffer.Length - 1) 
                : string.Empty;

            if (string.IsNullOrWhiteSpace(textBeforeTrigger))
                return null;

            // Get enabled snippets
            var snippets = _snippetManager.GetEnabled();

            // Find the longest matching keyword that ends the buffer
            Snippet? bestMatch = null;
            int bestMatchLength = 0;

            foreach (var snippet in snippets)
            {
                if (textBeforeTrigger.EndsWith(snippet.Keyword, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if this is a word boundary match
                    int keywordStart = textBeforeTrigger.Length - snippet.Keyword.Length;
                    
                    // If keyword is at the start or preceded by whitespace/punctuation, it's a valid match
                    if (keywordStart == 0 || 
                        char.IsWhiteSpace(textBeforeTrigger[keywordStart - 1]) ||
                        char.IsPunctuation(textBeforeTrigger[keywordStart - 1]))
                    {
                        if (snippet.Keyword.Length > bestMatchLength)
                        {
                            bestMatch = snippet;
                            bestMatchLength = snippet.Keyword.Length;
                        }
                    }
                }
            }

            if (bestMatch != null)
            {
                return new MatchResult
                {
                    Snippet = bestMatch,
                    KeywordLength = bestMatchLength,
                    TriggerChar = lastChar
                };
            }

            return null;
        }
    }

    /// <summary>
    /// Result of a snippet match operation
    /// </summary>
    public class MatchResult
    {
        public Snippet Snippet { get; set; } = null!;
        public int KeywordLength { get; set; }
        public char TriggerChar { get; set; }
    }
}
