using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Utils
{
    internal static class Helpers
    {
        public static string GetKeywordParts(string content, IEnumerable<string> keywords)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            var len = content.Length;
            const int maxLen = 30;
            if (len < 1 || keywords.Count() < 1)
            {
                return len > maxLen ? content.Substring(0, maxLen) + "..." : content;
            }

            var ranges = new List<Models.StrRange>();
            foreach (var keyword in keywords)
            {
                var range = GetRangeByKeyword(content, keyword);
                ranges.Add(range);
            }

            ranges = FixStrRange(ranges, content.Length);
            var parts = JoinRanges(len, ranges);
            var t = parts.Select(r => content.Substring(r.start, r.end - r.start)).ToList();
            var result = string.Join("...", t);
            result = PadResult(ranges, result, len);
            return result;
        }

        static string PadResult(List<Models.StrRange> ranges, string result, int len)
        {
            if (ranges.Count > 0)
            {
                if (ranges.First().start > 0)
                {
                    result = "..." + result;
                }
                if (ranges.Last().end < len)
                {
                    result = result + "...";
                }
            }

            return result;
        }

        static int CharWidth(char ch)
        {
            if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
            {
                return 1;
            }
            if (char.IsPunctuation(ch) || char.IsSeparator(ch))
            {
                return 1;
            }
            return 2;
        }

        static Models.StrRange GetRangeByKeyword(string content, string keyword)
        {
            var margin = 10;
            var idx = content.IndexOf(keyword);
            int start = idx;
            var i = 0;
            while (i < margin)
            {
                start = idx - i;
                if (start < 0)
                {
                    break;
                }
                var c = content[start];
                i += CharWidth(c);
            }
            var end = idx + keyword.Length;
            i = 0;
            while (i < margin)
            {
                end = idx + keyword.Length + i;
                if (end >= content.Length)
                {
                    break;
                }
                var c = content[end];
                i += CharWidth(c);
            }
            return new Models.StrRange(start, end);
        }

        public static List<Models.StrRange> FixStrRange(
            IEnumerable<Models.StrRange> ranges,
            int len
        )
        {
            return ranges
                .Select(r => r.Fix(len) ? r : null)
                .Where(r => r != null)
                .OrderBy(r => r.start)
                .ToList();
        }

        public static List<Models.StrRange> JoinRanges(int len, IEnumerable<Models.StrRange> ranges)
        {
            var result = new List<Models.StrRange>();
            if (len < 1)
            {
                return result;
            }

            Models.StrRange prev = null;
            foreach (var cur in ranges)
            {
                if (prev == null)
                {
                    prev = cur;
                    continue;
                }

                if (prev.end + 1 >= cur.start)
                {
                    prev.end = Math.Max(prev.end, cur.end);
                    continue;
                }

                result.Add(prev);
                prev = cur;
            }

            if (prev != null)
            {
                result.Add(prev);
            }
            return result;
        }

        public static string ToYmdHms(DateTime date)
        {
            return date.ToString(
                "yyyy-MM-dd hh:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture
            );
        }
    }
}
