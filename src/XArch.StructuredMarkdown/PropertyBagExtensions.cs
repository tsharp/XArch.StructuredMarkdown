using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using XArch.StructuredMarkdown.Serialization;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XArch.StructuredMarkdown
{
    public static class PropertyBagExtensions
    {
        public static string ToMarkdownFrontmatter(this PropertyBag propertyBag, bool includeSeparator = true)
        {
            if (!includeSeparator)
            {
                YamlSerializer.ToYaml(propertyBag);
            }

            return $"---\n{YamlSerializer.ToYaml(propertyBag)}---";
        }

        public static string ToXmlComment(this PropertyBag propertyBag, bool includeCommentBrackets = true)
        {
            var parts = new List<string>();

            foreach (var kv in propertyBag)
            {
                if (kv.Value is string s && s.StartsWith("[") && s.EndsWith("]"))
                {
                    // Already array-like string
                    parts.Add($"{kv.Key}: {s}");
                }
                else if (kv.Value is IEnumerable<object> list && !(kv.Value is string))
                {
                    // Generic object list (PropertyBag may use object arrays)
                    var strList = list.Select(x =>
                    {
                        if (x is string) return $"'{x}'";
                        return x?.ToString() ?? "null";
                    });
                    parts.Add($"{kv.Key}: [{string.Join(", ", strList)}]");
                }
                else if (kv.Value is IEnumerable<string> stringList)
                {
                    // Explicit string list
                    parts.Add($"{kv.Key}: [{string.Join(", ", stringList.Select(x => $"'{x}'"))}]");
                }
                else if (kv.Value is bool b)
                {
                    parts.Add($"{kv.Key}: {(b ? "true" : "false")}");
                }
                else if (kv.Value is null)
                {
                    parts.Add($"{kv.Key}: null");
                }
                else
                {
                    parts.Add($"{kv.Key}: {kv.Value}");
                }
            }

            string results = $"{string.Join(", ", parts)}";

            if (!includeCommentBrackets)
            {
                return results;
            }

            return $"<!-- {string.Join(", ", parts)} -->";
        }
    }
}
