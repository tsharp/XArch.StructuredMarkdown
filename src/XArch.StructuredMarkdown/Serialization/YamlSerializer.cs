using System;
using System.Collections.Generic;
using System.Text;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XArch.StructuredMarkdown.Serialization
{
    public static class YamlSerializer
    {
        static YamlSerializer()
        {
            Deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

            Serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                    .Build();
        }

        internal static readonly IDeserializer Deserializer;

        internal static readonly ISerializer Serializer;

        public static T? FromYaml<T>(this string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml))
            {
                return default;
            }

            return Deserializer.Deserialize<T>(yaml);
        }

        public static string ToYaml<T>(this T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return Serializer.Serialize(obj, obj.GetType());
        }

        public static string ToHtmlComment(this string yamlContent, bool includeCommentBrackets = true)
        {
            if (string.IsNullOrWhiteSpace(yamlContent))
                return "";

            var lines = yamlContent.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var sb = new StringBuilder();
            if (includeCommentBrackets)
                sb.Append("<!-- ");

            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    i++;
                    continue;
                }

                // List key (ends with ':') and next line(s) are list items
                if (line.EndsWith(":"))
                {
                    string key = line[..^1].Trim();
                    var listValues = new List<string>();
                    int j = i + 1;
                    while (j < lines.Length)
                    {
                        string nextLine = lines[j].Trim();
                        if (nextLine.StartsWith("-"))
                        {
                            string value = nextLine[1..].Trim();
                            if (!string.IsNullOrEmpty(value))
                                listValues.Add($"'{value}'");
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (listValues.Count > 0)
                    {
                        sb.Append($"{key}: [{string.Join(", ", listValues)}], ");
                        i = j;
                        continue;
                    }
                    else
                    {
                        sb.Append($"{key}: [], ");
                        i++;
                        continue;
                    }
                }
                // Normal key-value pair
                int colonIdx = line.IndexOf(':');
                if (colonIdx > -1)
                {
                    string key = line[..colonIdx].Trim();
                    string value = line[(colonIdx + 1)..].Trim();
                    sb.Append($"{key}: {value}, ");
                }
                i++;
            }

            // Remove trailing space and comma if present
            if (sb.Length > 2 && sb[^2] == ',')
                sb.Length -= 2;

            if (includeCommentBrackets)
                sb.Append(" -->");

            return sb.ToString().Trim();
        }
    }
}
