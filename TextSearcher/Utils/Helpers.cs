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

            const int margin = 8;
            var ranges = new List<Models.StrRange>();
            foreach (var keyword in keywords)
            {
                var idx = content.IndexOf(keyword);
                ranges.Add(new Models.StrRange(idx - margin, idx + keyword.Length + margin));
            }

            var parts = JoinRanges(len, ranges);
            var t = parts.Select(r => content.Substring(r.start, r.end - r.start)).ToList();
            var result = string.Join("...", t);
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

        public static List<Models.StrRange> JoinRanges(int len, IEnumerable<Models.StrRange> ranges)
        {
            var result = new List<Models.StrRange>();
            if (len < 1)
            {
                return result;
            }

            var t = ranges
                .Select(r => r.Fix(len) ? r : null)
                .Where(r => r != null)
                .OrderBy(r => r.start)
                .ToList();
            Models.StrRange prev = null;
            foreach (var cur in t)
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
