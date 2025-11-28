using System;

namespace RyCookieText.Models
{
    /// <summary>
    /// Type of snippet
    /// </summary>
    public enum SnippetType
    {
        Text,
        Code
    }

    /// <summary>
    /// Represents a text expansion snippet
    /// </summary>
    public class Snippet
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Group { get; set; } = "Default";
        public bool IsEnabled { get; set; } = true;
        public SnippetType Type { get; set; } = SnippetType.Text;
        public string? Language { get; set; } // For code snippets (e.g., "C#", "JavaScript", "Python")
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Snippet()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Validates the snippet data
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Keyword) && 
                   !string.IsNullOrWhiteSpace(Content);
        }

        /// <summary>
        /// Creates a copy of this snippet
        /// </summary>
        public Snippet Clone()
        {
            return new Snippet
            {
                Id = this.Id,
                Keyword = this.Keyword,
                Content = this.Content,
                Group = this.Group,
                IsEnabled = this.IsEnabled,
                Type = this.Type,
                Language = this.Language,
                CreatedDate = this.CreatedDate,
                ModifiedDate = this.ModifiedDate
            };
        }
    }
}
