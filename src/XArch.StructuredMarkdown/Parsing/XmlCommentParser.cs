using System;
using System.Text;

using XArch.StructuredMarkdown.Serialization;

namespace XArch.StructuredMarkdown.Parsing
{
    public static class XmlCommentParser
    {
        public static XmlCommentKind TryParseMetadata(string metadata, out PropertyBag? propertyBag)
        {
            propertyBag = null;
            
            if (string.IsNullOrWhiteSpace(metadata))
            {
                return XmlCommentKind.Empty;
            }

            if (metadata.StartsWith("/section", StringComparison.OrdinalIgnoreCase))
            {
                return XmlCommentKind.EndSection;
            }

            try
            {
                propertyBag = ParseMetadata(metadata);

                if (propertyBag == null)
                {
                    return XmlCommentKind.Empty;
                }

                if (propertyBag.ContainsKey("section"))
                {
                    return XmlCommentKind.StartSection;
                }

                return XmlCommentKind.SimpleMetadata;
            }
            catch (FormatException)
            {
                propertyBag = null;
                return XmlCommentKind.Error;
            }
        }

        public static PropertyBag? ParseMetadata(string metadata)
        {
            if (string.IsNullOrWhiteSpace(metadata))
            {
                return null;
            }

            StringBuilder metadataBuilder = new StringBuilder();
            bool isInArray = false;
            bool isInProperty = false;
            int depth = 0;

            for (int i = 0; i < metadata.Length; i++)
            {
                if (char.IsWhiteSpace(metadata[i]) && !isInProperty)
                {
                    continue;
                }

                if (metadata[i] == ',' && !isInArray)
                {
                    metadataBuilder.AppendLine();
                    isInProperty = false;
                    continue;
                }

                if (metadata[i] == ':' && !isInProperty)
                {
                    isInProperty = true;
                }

                if (metadata[i] == '[')
                {
                    isInArray = true;
                    depth++;
                }

                if (metadata[i] == ']')
                {
                    if (depth == 0)
                    {
                        throw new FormatException("Unmatched closing bracket in metadata.");
                    }

                    depth--;

                    if (depth == 0)
                    {
                        isInArray = false;
                    }
                }

                metadataBuilder.Append(metadata[i]);
            }

            string metadataString = metadataBuilder.ToString().Trim();

            return metadataString.FromYaml<PropertyBag>();
        }
    }
}
