using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Ruminoid.Common2.Utils.Text
{
    [PublicAPI]
    public static class TextUtils
    {
        public static readonly Regex EngOrNumCharRegex = new("^[a-zA-Z0-9]+$");

        public static bool IsEngOrNumChar(this string str) =>
            EngOrNumCharRegex.IsMatch(str);
    }
}
