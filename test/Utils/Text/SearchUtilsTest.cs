using System.Collections.Generic;
using System.Linq;
using Ruminoid.Common2.Utils.Text;
using Xunit;

namespace Ruminoid.Common2.Test.Utils.Text
{
    public static class SearchUtilsTest
    {
        [Fact]
        public static void GetTextForSearchTextTest()
        {
            List<string> expected = new()
            {
                // ReSharper disable StringLiteralTypo

                "aafangshi",
                "aafangs",
                "aafshi",
                "aafs"

                // ReSharper restore StringLiteralTypo
            };

            List<string> actual = SearchUtils.GetTextForSearch("AA方式");

            Assert.True(actual.All(expected.Contains));
            Assert.Equal(expected.Count, actual.Count);
        }

        [Fact]
        public static void SearchTest()
        {
            // ReSharper disable StringLiteralTypo

            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "aafa").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "aafangs").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "aafangsh").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "aafsh").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "afsh").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "fsh").Any());
            Assert.True(SearchUtils.Search(new[] { "AA方式" }, "fangsh").Any());
            Assert.False(SearchUtils.Search(new[] { "AA方式" }, "bb").Any());
            Assert.False(SearchUtils.Search(new[] { "AA方式" }, "aafangg").Any());
            Assert.False(SearchUtils.Search(new[] { "AA方式" }, "aafngshi").Any());

            // ReSharper restore StringLiteralTypo
        }
    }
}
