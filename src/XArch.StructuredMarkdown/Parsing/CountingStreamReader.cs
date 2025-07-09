using System;
using System.IO;

namespace XArch.StructuredMarkdown.Parsing
{
    internal class LineCountingStreamReader(StreamReader streamReader, bool leaveOpen = false) : IDisposable
    {
        public int LineNumber { get; private set; } = 0;

        public void Dispose()
        {
            if (!leaveOpen)
            {
                streamReader?.Dispose();
            }
        }

        public string? ReadLine(bool skipEmptyLines = false)
        {
            string? line;

            if (skipEmptyLines)
            {
                line = this.SkipEmptyLines();
            }
            else
            {
                line = streamReader.ReadLine();
                LineNumber++;
            }

            return line;
        }

        private string? SkipEmptyLines()
        {
            string? line;

            while ((line = this.ReadLine()) != null && string.IsNullOrWhiteSpace(line))
            {
            }

            return line;
        }
    }
}
