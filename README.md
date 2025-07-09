<!-- section: id=README.md, tags=[nuget, package, documentation, structured-markdown] -->
# XArch.StructuredMarkdown
A .NET library for working with **Simple Structured Markdown**, enabling structured document management with metadata and visibility control.

## Overview
`XArch.StructuredMarkdown` provides tools to parse and serialize documents that follow the [Structured Markdown Format](https://github.com/tsharp/XArch.StructuredMarkdown/blob/main/docs/SPEC.md). This format enables:

- **Block-level section tags** for grouping content with arbitrary metadata.
- **Inline metadata comments** for fine-grained control over visibility and properties.
- **Tag comments** for data tagging within session logs or summaries.

## Features
### 1. **Parsing**
- Parse documents using a stack-based approach, tracking current metadata context (visibility, tags, etc.).
- Supports both section tags and inline metadata comments.

### 2. **Serialization**
- Generate structured markdown with proper formatting for sections, inline comments, and tag comments.
- Preserve unknown metadata keys during round-trip editing.

## Usage
Install the package via NuGet:

```bash
dotnet add package XArch.StructuredMarkdown
```

### Example: Parsing a Document
```csharp
using XArch.StructuredMarkdown;

var document = StructuredMarkdownParser.Parse("path/to/document.md");
```

### Example: Serializing a Document
```csharp
using XArch.StructuredMarkdown;

string markdown = StructuredMarkdownSerializer.ToMarkdown(document);
File.WriteAllText("output.md", markdown);
```

## Format Specification
For full details on the format, see the [Markdown Content Partitioning and Tagging Spec](https://github.com/tsharp/XArch.StructuredMarkdown/blob/main/docs/SPEC.md).

---

## Contributing
Contributions are welcome! Please open an issue or submit a pull request.

## License
Apache 2.0 License. See `LICENSE` for more information.

<!-- /section -->