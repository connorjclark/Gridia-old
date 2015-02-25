// Todo: Parsing for color

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gridia
{
    public class RichText
    {
        public static bool HtmlIsValid(String html)
        {
            var numOpeningBoldTags = html.Split(new[] { "<b>" }, StringSplitOptions.None).Length;
            var numClosingBoldTags = html.Split(new[] { "</b>" }, StringSplitOptions.None).Length;
            var numOpeningItalicsTags = html.Split(new[] { "<i>" }, StringSplitOptions.None).Length;
            var numClosingItalicsTags = html.Split(new[] { "</i>" }, StringSplitOptions.None).Length;
            return numOpeningBoldTags == numClosingBoldTags && numOpeningItalicsTags == numClosingItalicsTags;
        }

        public int MaxLength { get; set; }
        private String _text;
        private readonly Queue<String> _entries = new Queue<String>(); 

        public RichText()
        {
            MaxLength = -1;
        }

        private String ReplaceFirstOccurrence(String source, String find, String replace)
        {
            var place = source.IndexOf(find, StringComparison.Ordinal);
            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public void Append(String text)
        {
            var entry = Parse(text);
            _entries.Enqueue(entry);
            _text += Parse(text);
            if (MaxLength > 0 && _text.Length > MaxLength)
            {
                _text = ReplaceFirstOccurrence(_text, _entries.Dequeue(), "");
            }
        }

        private String ParseBold(String text)
        {
            return Regex.Replace(text, @"(?<!\\)\*(.*)(?<!\\)\*", "<b>$1</b>");
        }

        private String ParseItalics(String text)
        {
            return Regex.Replace(text, @"\*\*(.*)\*\*", "<i>$1</i>");
        }

        private String UnescapeAsterisk(String text)
        {
            return Regex.Replace(text, @"\\\*", "*");
        }

        private String Parse(String text)
        {
            return UnescapeAsterisk(ParseBold(ParseItalics(text)));
        }

        public override String ToString()
        {
            return _text;
        }
    }
}
