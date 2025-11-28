using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RyCookieText.Models;

namespace RyCookieText.Services
{
    /// <summary>
    /// Manages CRUD operations for snippets with JSON persistence
    /// </summary>
    public class SnippetManager
    {
        private readonly string _filePath;
        private List<Snippet> _snippets;
        private readonly object _lock = new object();

        public event EventHandler? SnippetsChanged;

        public SnippetManager(string filePath)
        {
            _filePath = filePath;
            _snippets = new List<Snippet>();
            Load();
        }

        /// <summary>
        /// Gets all snippets
        /// </summary>
        public List<Snippet> GetAll()
        {
            lock (_lock)
            {
                return _snippets.Select(s => s.Clone()).ToList();
            }
        }

        /// <summary>
        /// Gets enabled snippets only
        /// </summary>
        public List<Snippet> GetEnabled()
        {
            lock (_lock)
            {
                return _snippets.Where(s => s.IsEnabled).Select(s => s.Clone()).ToList();
            }
        }

        /// <summary>
        /// Gets a snippet by ID
        /// </summary>
        public Snippet? GetById(Guid id)
        {
            lock (_lock)
            {
                return _snippets.FirstOrDefault(s => s.Id == id)?.Clone();
            }
        }

        /// <summary>
        /// Adds a new snippet
        /// </summary>
        public bool Add(Snippet snippet)
        {
            if (!snippet.IsValid())
                return false;

            lock (_lock)
            {
                // Check for duplicate keyword
                if (_snippets.Any(s => s.Keyword.Equals(snippet.Keyword, StringComparison.OrdinalIgnoreCase)))
                    return false;

                snippet.Id = Guid.NewGuid();
                snippet.CreatedDate = DateTime.Now;
                snippet.ModifiedDate = DateTime.Now;
                _snippets.Add(snippet);
            }

            Save();
            SnippetsChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Updates an existing snippet
        /// </summary>
        public bool Update(Snippet snippet)
        {
            if (!snippet.IsValid())
                return false;

            lock (_lock)
            {
                var existing = _snippets.FirstOrDefault(s => s.Id == snippet.Id);
                if (existing == null)
                    return false;

                // Check for duplicate keyword (excluding current snippet)
                if (_snippets.Any(s => s.Id != snippet.Id && 
                                      s.Keyword.Equals(snippet.Keyword, StringComparison.OrdinalIgnoreCase)))
                    return false;

                existing.Keyword = snippet.Keyword;
                existing.Content = snippet.Content;
                existing.Group = snippet.Group;
                existing.IsEnabled = snippet.IsEnabled;
                existing.ModifiedDate = DateTime.Now;
            }

            Save();
            SnippetsChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Deletes a snippet
        /// </summary>
        public bool Delete(Guid id)
        {
            bool removed;
            lock (_lock)
            {
                removed = _snippets.RemoveAll(s => s.Id == id) > 0;
            }

            if (removed)
            {
                Save();
                SnippetsChanged?.Invoke(this, EventArgs.Empty);
            }

            return removed;
        }

        /// <summary>
        /// Searches snippets by keyword or content
        /// </summary>
        public List<Snippet> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAll();

            lock (_lock)
            {
                return _snippets
                    .Where(s => s.Keyword.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               s.Content.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               s.Group.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Select(s => s.Clone())
                    .ToList();
            }
        }

        /// <summary>
        /// Loads snippets from JSON file
        /// </summary>
        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    var snippets = JsonConvert.DeserializeObject<List<Snippet>>(json);
                    
                    lock (_lock)
                    {
                        _snippets = snippets ?? new List<Snippet>();
                    }
                }
                else
                {
                    // Create default snippets
                    CreateDefaultSnippets();
                    Save();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading snippets: {ex.Message}");
                CreateDefaultSnippets();
            }
        }

        /// <summary>
        /// Saves snippets to JSON file
        /// </summary>
        private void Save()
        {
            try
            {
                string json;
                lock (_lock)
                {
                    json = JsonConvert.SerializeObject(_snippets, Formatting.Indented);
                }
                
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving snippets: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates default sample snippets
        /// </summary>
        private void CreateDefaultSnippets()
        {
            lock (_lock)
            {
                _snippets = new List<Snippet>
                {
                    new Snippet
                    {
                        Keyword = "btw",
                        Content = "by the way",
                        Group = "Common"
                    },
                    new Snippet
                    {
                        Keyword = "omw",
                        Content = "on my way",
                        Group = "Common"
                    },
                    new Snippet
                    {
                        Keyword = "brb",
                        Content = "be right back",
                        Group = "Common"
                    },
                    new Snippet
                    {
                        Keyword = "addr",
                        Content = "123 Main Street\nCity, State 12345",
                        Group = "Personal"
                    },
                    new Snippet
                    {
                        Keyword = "sig",
                        Content = "Best regards,\nYour Name\nyour.email@example.com",
                        Group = "Email"
                    }
                };
            }
        }
    }
}
