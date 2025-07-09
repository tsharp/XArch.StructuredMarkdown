# Markdown Content Partitioning and Tagging Spec

## **Version**

**Spec Version:** 0.1  
**Last Updated:** 2025-07-02

## Purpose

To provide a consistent, human-editable, and machine-parseable format for managing visibility, metadata, and structure of campaign data in Markdown documents, suitable for use in structured markdown systems.

---

## Format Overview
- Supports both **inline metadata comments** (for headings/paragraphs/lists) and **block-level section tags** (with nesting, open/close).    
- Allows for fine-grained access control, data tagging, and chunked retrieval for LLM/RAG pipelines.    
- All metadata is stored in HTML-style comments to remain Markdown-compatible.    

---

## Elements and Rules

### 1. **Section Tags (Block-Level, Nestable)**
**Open Tag:**
`<!-- section: key1=value1; key2=value2; ... -->`
- Begins a block with arbitrary metadata as key-value pairs (semicolon-separated).    
- Supports any keys, e.g. `kind`, `visible_to`, `npc`, `location`, `session`, `source`.    

**Close Tag:**
`<!-- /section -->`
- Ends the most recently opened section.    

**Nesting:**  
Sections may be nested arbitrarily. Attributes of nested sections can override or supplement those of parent sections.

**Example:**
```
<!-- section: kind=dm_notes; visible_to=dm -->
Hidden notes for the DM.
<!-- section: kind=secret; visible_to=author -->
[Author only]
<!-- /section -->
More DM info here.
<!-- /section -->
```

---

### 2. **Inline Metadata Comments (Line or Block Scope)**

- HTML-style comments placed at the end of a heading, or at the start of a paragraph/list.  

**Syntax:**
`# Section A <!-- visible_to: [dm, author] -->`
or
`<!-- visible_to: [dm] --> Paragraph text...`

**Scope Rules:**
- **After a heading:**  
    Applies to the entire section (all content until the next heading of the same or higher level).
- **At start of paragraph or list:**  
    Applies to that paragraph or list only.
---

### 3. **Tag Comments (Line/Paragraph Scope)**

- Use for data tagging within session logs or summaries, e.g.:    
    `<!-- tag: party_npc_interaction; npc=Keshra; location=Galeport; session=20 -->`
- Applies to the immediately following line or paragraph.

---

## **Visibility and Metadata Precedence**
- **Section tags** (block-level) take precedence over inline comments for content within their boundaries.
- **Inline tags** apply only if not overridden by an enclosing section’s metadata.
- **When multiple visibility rules overlap, the most restrictive applies** (e.g., visible_to: dm is more restrictive than visible_to: all).

---
## **Parser Guidelines**
- Parse sections using a stack. Each open tag pushes metadata, each close tag pops.
- Track current metadata context (including visibility, kind, tags, etc.) at each point in the file.
- Inline comments update context for the next block, unless overridden by a section tag.
- Tag comments are available for data queries and indexing.

---

## **Example Document**
```
# Galeport NPCs <!-- visible_to: [dm] -->
- Keshra: merchant in the city
- Fira: tavern keeper

# Player Info <!-- visible_to: [player] -->
Party learns about the haunted ruins.

<!-- section: kind=party_npc_interaction; npc=Fira; location=Galeport; session=21; source=session-21-log -->
The party met Fira at the tavern and heard about a suspicious stranger.

<!-- section: kind=dm_notes; visible_to=dm -->
Fira is secretly a Circle of Dawns informant.
<!-- /section -->

<!-- /section -->
```

---

## **Extensibility**
- Any key-value pairs may be added for future features (e.g., `source_doc`, `timestamp`, `author`, `tags`, etc).
- Tools should ignore unknown keys but preserve them for round-trip editing.

---

## **Rationale**
- **Human-friendly:** Easy to write/edit in Markdown.
- **Machine-parseable:** Deterministic, stack-based parsing.
- **Flexible:** Supports both coarse and fine partitioning, arbitrary metadata, and access control.
- **LLM/RAG-ready:** Enables easy chunking, search, and retrieval based on context or access.

---