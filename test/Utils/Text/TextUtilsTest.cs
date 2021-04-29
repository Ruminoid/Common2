using Ruminoid.Common2.Utils.Text;
using Xunit;

namespace Ruminoid.Common2.Test.Utils.Text
{
    public static class TextUtilsTest
    {
        [Fact]
        public static void EngOrNumCharTest()
        {
            Assert.True("abc".IsEngOrNumChar());
            Assert.True("123".IsEngOrNumChar());
            Assert.True("123abc".IsEngOrNumChar());
            Assert.False("123abc;".IsEngOrNumChar());
            Assert.False("123abc我".IsEngOrNumChar());
        }
    }
}
