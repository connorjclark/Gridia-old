// Todo: Parsing for color

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gridia
{
    public class RichText
    {
        private const String AsteriskLookback = @"(?<!\\)\*";
        private static readonly String BoldPattern = String.Format("{0}(.+?){0}", AsteriskLookback);
        private static readonly String ItalicsPattern = String.Format("{0}{0}(.+?){0}{0}", AsteriskLookback);
        private static readonly String BothPattern = String.Format("{0}{0}{0}(.+?){0}{0}{0}", AsteriskLookback);

        public static bool HtmlIsValid(String html)
        {
            Func<String, int> countTags = tag => html.Split(new[] {tag}, StringSplitOptions.None).Length;
            Func<String, bool> tagIsBalanced = tag => countTags(tag) == countTags(tag.Insert(1, "/"));
            return tagIsBalanced("<b>") && tagIsBalanced("<i>") && tagIsBalanced("<color>");
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
            return Regex.Replace(text, BoldPattern, "<b>$1</b>");
        }

        private String ParseItalics(String text)
        {
            return Regex.Replace(text, ItalicsPattern, "<i>$1</i>");
        }

        // :(
        private String ParseBoth(String text)
        {
            return Regex.Replace(text, BothPattern, "<i><b>$1</b></i>");
        }

        private String UnescapeAsterisk(String text)
        {
            return Regex.Replace(text, @"\\\*", "*");
        }

        private String Parse(String text)
        {
            return UnescapeAsterisk(ParseBold(ParseItalics(ParseBoth(text))));
        }

        public override String ToString()
        {
            return _text;
        }
    }
}
