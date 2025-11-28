using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using RyCookieText.Models;

namespace RyCookieText.Services
{
    /// <summary>
    /// Service for importing snippets from CSV files
    /// </summary>
    public class CsvImportService
    {
        public class CsvImportResult
        {
            public int SuccessCount { get; set; }
            public int SkippedCount { get; set; }
            public int ErrorCount { get; set; }
            public List<string> Errors { get; set; } = new();
            public List<Snippet> ImportedSnippets { get; set; } = new();
        }

        public class SnippetCsvRecord
        {
            public string Keyword { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public string Group { get; set; } = "Default";
            public string Type { get; set; } = "text";
            public string IsEnabled { get; set; } = "true";
        }

        /// <summary>
        /// Import snippets from a CSV file
        /// </summary>
        public CsvImportResult ImportFromCsv(string filePath, bool skipDuplicates = true)
        {
            var result = new CsvImportResult();

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                });

                var records = csv.GetRecords<SnippetCsvRecord>().ToList();

                foreach (var record in records)
                {
                    try
                    {
                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(record.Keyword))
                        {
                            result.ErrorCount++;
                            result.Errors.Add($"Row skipped: Missing keyword");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(record.Content))
                        {
                            result.ErrorCount++;
                            result.Errors.Add($"Row skipped: Missing content for keyword '{record.Keyword}'");
                            continue;
                        }

                        // Parse snippet type
                        SnippetType snippetType = SnippetType.Text;
                        if (!string.IsNullOrWhiteSpace(record.Type))
                        {
                            if (record.Type.Equals("code", StringComparison.OrdinalIgnoreCase))
                            {
                                snippetType = SnippetType.Code;
                            }
                        }

                        // Parse IsEnabled
                        bool isEnabled = true;
                        if (!string.IsNullOrWhiteSpace(record.IsEnabled))
                        {
                            bool.TryParse(record.IsEnabled, out isEnabled);
                        }

                        // Create snippet
                        var snippet = new Snippet
                        {
                            Keyword = record.Keyword.Trim(),
                            Content = record.Content,
                            Group = string.IsNullOrWhiteSpace(record.Group) ? "Default" : record.Group.Trim(),
                            Type = snippetType,
                            IsEnabled = isEnabled
                        };

                        result.ImportedSnippets.Add(snippet);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Error processing row: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                result.Errors.Add($"Failed to read CSV file: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Export snippets to a CSV file
        /// </summary>
        public bool ExportToCsv(string filePath, IEnumerable<Snippet> snippets)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                var records = snippets.Select(s => new SnippetCsvRecord
                {
                    Keyword = s.Keyword,
                    Content = s.Content,
                    Group = s.Group,
                    Type = s.Type.ToString().ToLower(),
                    IsEnabled = s.IsEnabled.ToString().ToLower()
                });

                csv.WriteRecords(records);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
