using System;
using System.Collections.Generic;
using System.Linq;
using Ruminoid.Common2.Utils.Text;
using Xunit;
using Xunit.Abstractions;

namespace Ruminoid.Common2.Test.Utils.Text
{
    public class SearchUtilsTest
    {
        #region Data

        // ReSharper disable StringLiteralTypo

        private static readonly List<string> Expected = new()
        {
            "aafangshi",
            "aafangs",
            "aafang式",
            "aafshi",
            "aafs",
            "aaf式",
            "aa方shi",
            "aa方s",
            "aa方式"
            
        };

        private static readonly List<string> SearchDataA = new()
        {
            "aafa",
            "aafangs",
            "aafangsh",
            "aafsh",
            "afsh",
            "fsh"
        };

        private static readonly List<string> SearchDataB = new()
        {
            "方sh",
            "A方式"
        };

        private static readonly List<string> SearchDataC = new()
        {
            "bb",
            "aafangg",
            "aafngshi",
            "啊",
            "房十",
            "a房s"
        };

        // ReSharper restore StringLiteralTypo

        #endregion

        [Fact]
        [Obsolete]
        public static void GetTextForSearchTextTest()
        {
            List<string> actual = SearchUtils.GetTextForSearch("AA方式");
            Assert.True(actual.All(Expected.Contains));
            Assert.Equal(Expected.Count, actual.Count);
        }

        [Fact]
        [Obsolete]
        public static void ObsoleteSearchTest()
        {
            foreach (string s in SearchDataA) Assert.True(SearchUtils.Search(new[] {"AA方式"}, s).Any());
            foreach (string s in SearchDataC) Assert.False(SearchUtils.Search(new[] { "AA方式" }, s).Any());
        }

        private static bool SearchResultValidB(
            List<string> expected,
            HashSet<string> actual)
        {
            return (actual.All(expected.Contains)) &&
                   expected.Count == actual.Count;
        }

        private static bool SearchResultValid(
            HashSet<string> results,
            string resultValue = "SEARCH_REAULT_A") =>
            results.Count == 1 &&
            results.First() == resultValue;

        private readonly ITestOutputHelper _output;

        public SearchUtilsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SearchTest()
        {
            SearchTrie<string> trie = new(new()
            {
                ("AA方式", "SEARCH_REAULT_A"),
                ("AB模式", "SEARCH_REAULT_B"),
                ("AA房十", "SEARCH_REAULT_C")
            });

            HashSet<string> result = trie.Search("aafshi");

            // 拼音/首字母/同汉字搜索测试
            foreach (string s in SearchDataA)
            {
                Assert.True(SearchResultValidB(new List<string>() {"SEARCH_REAULT_A", "SEARCH_REAULT_C"},
                    trie.Search(s)));
                _output.WriteLine(s + " passed.");
            }
            foreach (string s in SearchDataB) Assert.True(SearchResultValidB(new List<string>() { "SEARCH_REAULT_A" }, trie.Search(s)));
            foreach (string s in SearchDataC) Assert.False(SearchResultValid(trie.Search(s)));

            // 不同汉字搜索测试
            Assert.False(SearchResultValid(trie.Search("AA房十")));
            Assert.True(SearchResultValid(trie.Search("AA房十"), "SEARCH_REAULT_C"));
        }
    }
}
