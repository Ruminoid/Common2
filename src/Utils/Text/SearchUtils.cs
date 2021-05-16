using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.International.Converters.PinYinConverter;

namespace Ruminoid.Common2.Utils.Text
{
    [PublicAPI]
    public static class SearchUtils
    {
        public static List<string> GetTextForSearch(string text)
        {
            List<List<string>> textList = new();

            foreach (char c in text)
            {
                if (ChineseChar.IsValidChar(c))
                {
                    // Chinese char

                    List<string> strList =
                        new ChineseChar(c).Pinyins
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => Regex.Replace(x, @"\d", "").ToLower())
                            .Distinct()
                            .ToList();

                    strList.AddRange(
                        strList
                            .Select(x => x[..1])
                            .ToArray());

                    textList.Add(strList);
                }
                else if (c.ToString().IsEngOrNumChar())
                {
                    // Eng or num

                    textList.Add(new List<string> {c.ToString().ToLower()});
                }

                // Symbols, continue
            }

            return textList.Aggregate(
                new List<string>(),
                (current, list) =>
                    current.DefaultIfEmpty()
                        .SelectMany(x =>
                            list.Select(y =>
                                (x ?? "") + y))
                        .ToList());
        }

        public static List<string> Search(IEnumerable<string> items, string searchText) =>
            Search(items, x => x, searchText);

        public static List<T> Search<T>(IEnumerable<T> items, Func<T, string> text, string searchText)
        {
            searchText = searchText.ToLower();

            return items
                .Where(x => GetTextForSearch(text(x))
                    .Any(y => y.Contains(searchText)))
                .ToList();
        }
    }
}
