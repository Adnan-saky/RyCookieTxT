# CSV Import Format for RyCookie Text

This document describes the CSV format for importing snippets into RyCookie Text.

## CSV Format Specification

The CSV file should have the following columns:

| Column | Required | Description | Example Values |
|--------|----------|-------------|----------------|
| `Keyword` | Yes | The trigger keyword for the snippet | `btw`, `addr`, `sig` |
| `Content` | Yes | The expanded text content | `by the way`, `123 Main St` |
| `Group` | No | Category/group for organization | `Common`, `Work`, `Personal` |
| `Type` | No | Snippet type: `text` or `code` | `text`, `code` |
| `IsEnabled` | No | Whether snippet is active | `true`, `false` |

## Example CSV File

```csv
Keyword,Content,Group,Type,IsEnabled
btw,by the way,Common,text,true
omw,on my way,Common,text,true
addr,"123 Main Street
Anytown, ST 12345",Personal,text,true
sig,"Best regards,
John Doe
john@example.com",Work,text,true
mycode,console.log("Hello World");,JavaScript,code,true
pyprint,print(f"Value: {value}"),Python,code,true
```

## Important Notes

1. **Multi-line Content**: Wrap multi-line content in double quotes
2. **Commas in Content**: Wrap content containing commas in double quotes
3. **Default Values**:
   - `Group`: "Default" if not specified
   - `Type`: "text" if not specified
   - `IsEnabled`: "true" if not specified
4. **Header Row**: The first row must contain column headers
5. **Encoding**: Use UTF-8 encoding for special characters

## Importing Snippets

1. Open RyCookie Text
2. Click the "📥 Import CSV" button
3. Select your CSV file
4. Review the import results
5. Imported snippets will be added to your collection

## Error Handling

- Rows with missing `Keyword` or `Content` will be skipped
- Duplicate keywords will be skipped (existing snippets won't be overwritten)
- Invalid rows will be reported in the import results dialog

## Exporting Snippets

You can also export your current snippets to CSV:

1. Click the "📤 Export CSV" button
2. Choose a location to save the file
3. The exported CSV can be imported later or shared with others
