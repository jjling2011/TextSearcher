using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TextSearcher.Utils;

namespace TextSearcherTests
{
    [TestClass]
    public class HelpersTest
    {
        [DataTestMethod]
        [DataRow("fox jumps 棕毛", "...k brown fox jumps over th...og 那只敏捷的棕毛狐狸跃过那只懒狗")]
        [DataRow("The 懒狗", "the quick b...棕毛狐狸跃过那只懒狗")]
        public void GetKeywordPartsTest(string keywords, string expected)
        {
            const string src = "The quick brown fox jumps over the lazy dog 那只敏捷的棕毛狐狸跃过那只懒狗";
            var kws = keywords.ToLower().Split(' ').ToList();
            var result = Helpers.GetKeywordParts(src.ToLower(), kws);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(10, "1,2;3,4;", "1,4;")]
        [DataRow(10, "2,2;3,0;1,4;7,8;8,9;", "0,4;7,9")]
        public void JoinRangesTest(int len, string source, string result)
        {
            int num(string s)
            {
                if (int.TryParse(s, out var n))
                {
                    return n;
                }
                return 0;
            }

            List<TextSearcher.Models.StrRange> split(string str)
            {
                return str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(ss => new TextSearcher.Models.StrRange(num(ss[0]), num(ss[1])))
                    .ToList();
            }

            var ranges = split(source);
            var expected = split(result);

            var join = TextSearcher.Utils.Helpers.JoinRanges(len, ranges);
            Assert.AreEqual(expected.Count, join.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].start, join[i].start);
                Assert.AreEqual(expected[i].end, join[i].end);
            }
        }
    }
}
