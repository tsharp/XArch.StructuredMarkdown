using System;

namespace XArch.StructuredMarkdown.Parsing
{
    public static class XmlCommentExtractor
    {
        private static void AssertCleanOutput(this string output)
        {
            if (output.Contains("<!--", StringComparison.Ordinal) ||
                output.Contains("-->", StringComparison.Ordinal))
            {
                throw new FormatException("Nested or improperly closed XML comments found in input string.");
            }
        }

        private static void ExtractXmlComment(this string input, out string? output, out string? comment)
        {
            if (string.IsNullOrEmpty(input))
            {
                output = null;
                comment = null;
                return;
            }

            // Find the first HTML comment start <!--
            int startIndex = input.IndexOf("<!--", StringComparison.Ordinal);

            if (startIndex == -1)
            {
                output = input.Trim();
                comment = null;
                output.AssertCleanOutput();

                return;
            }

            // Find the corresponding end -->, starting from the beginning of the comment
            int endIndex = input.IndexOf("-->", startIndex, StringComparison.Ordinal);

            if (endIndex == -1)
            {
                throw new FormatException("Unclosed XML comment found in input string.");
            }

            // Extract the comment content (excluding <!-- and -->)
            string extractedComment = input.Substring(startIndex + 4, endIndex - startIndex - 4).Trim();

            // Remove the comment from the original input
            string remainingInput = input.Remove(startIndex, endIndex - startIndex + 3);

            output = remainingInput.Trim();
            comment = extractedComment;

            output.AssertCleanOutput();

            return;
        }

        public static XmlCommentExtractorResult TryExtractXmlComment(this string input, out string? output, out string? comment)
        {
            try
            {
                input.ExtractXmlComment(out output, out comment);

                if (string.IsNullOrWhiteSpace(comment))
                {
                    return XmlCommentExtractorResult.Empty;
                }

                return XmlCommentExtractorResult.Comment;
            }
            catch (FormatException)
            {
                output = null;
                comment = null;

                return XmlCommentExtractorResult.Error;
            }
        }
    }
}
