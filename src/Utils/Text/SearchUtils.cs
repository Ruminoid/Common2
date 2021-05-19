using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.International.Converters.PinYinConverter;

namespace Ruminoid.Common2.Utils.Text
{
    [PublicAPI]
    public class SearchTrie<TValue>
    {
        public class Node
        {
            private HashSet<TValue> _word;

            public HashSet<TValue> Word => _word ??= new();

            public bool IsTerminal => Edges.Count == 0 && Word != null;

            public Dictionary<string, Node> Edges = new();
        }

        public Node Root = new();

        #region Utils

        public static List<string> GenerateCharSearchPattern(char c)
        {
            List<string> result = new();

            if (ChineseChar.IsValidChar(c))
            {
                // Chinese char

                result.AddRange(
                    new ChineseChar(c).Pinyins
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => Regex.Replace(x, @"\d", "").ToLower())
                        .Distinct()
                        .ToList()); // 全拼

                result.AddRange(
                    result
                        .Select(x => x[..1]) // 首字母
                        .ToArray());

                result.Add(c.ToString()); // 原字符
            }
            else if (c.ToString().IsEngOrNumChar())
            {
                // Eng or num

                result.Add(c.ToString().ToLower());
                result.Add(c.ToString().ToUpper());
            }

            return result;
        }

        #endregion

        public SearchTrie(List<(string Key, TValue Value)> words)
        {
            for (int w = 0; w < words.Count; w++)
            {
                var word = words[w];
                List<Node> nodes = new() {Root};
                for (int len = 1; len <= word.Key.Length; len++)
                {
                    var letter = word.Key[len - 1];

                    List<string> searchPattern = GenerateCharSearchPattern(letter);

                    if (!searchPattern.Any()) continue;

                    List<Node> nexts = new();

                    foreach (Node node in nodes)
                    {
                        foreach (string s in searchPattern)
                        {
                            if (!node.Edges.ContainsKey(s))
                                node.Edges[s] = new();

                            nexts.Add(node.Edges[s]);
                        }
                    }

                    //foreach (string s in searchPattern)
                    //{
                    //    if (node.Edges.ContainsKey(s))
                    //        next = node.Edges[s];
                    //}

                    //foreach (string s in searchPattern)
                    //    if (node.Edges.ContainsKey(s))
                    //        next = node.Edges[s];

                    //if (next is null) next = new();

                    //foreach (string s in searchPattern)
                    //{
                    //    if (!node.Edges.TryGetValue(s, out next))
                    //    {
                    //        next = new Node();

                    //        node.Edges.Add(s, next);
                    //    }
                    //}

                    if (len == word.Key.Length)
                    {
                        foreach (Node next in nexts)
                        {
                            next.Word.Add(word.Value);
                        }
                    }

                    nodes = nexts;
                }
            }
        }
    }

    [PublicAPI]
    public static class SearchUtils
    {
        [Obsolete]
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
                            .ToList(); // 全拼

                    strList.AddRange(
                        strList
                            .Select(x => x[..1]) // 首字母
                            .ToArray());

                    strList.Add(c.ToString()); // 原字符

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
                                x + y))
                        .ToList());
        }

        [Obsolete]
        public static List<string> Search(IEnumerable<string> items, string searchText) =>
            Search(items, x => x, searchText);

        [Obsolete]
        public static List<T> Search<T>(IEnumerable<T> items, Func<T, string> text, string searchText)
        {
            searchText = searchText.ToLower();

            return items
                .Where(x => GetTextForSearch(text(x))
                    .Any(y => y.Contains(searchText)))
                .ToList();
        }

        private static void CollectAllValues<T>(
            SearchTrie<T>.Node node,
            HashSet<T> result)
        {
            if (node.IsTerminal)
            {
                foreach (T w in node.Word)
                {
                    result.Add(w);
                }
            }

            foreach (var edge in node.Edges)
            {
                CollectAllValues(edge.Value, result);
            }
        }

        private static void SearchIntl<T>(
            SearchTrie<T>.Node node,
            string searchText,
            HashSet<T> result)
        {
            foreach (var edge in node.Edges)
            {
                if (edge.Key.StartsWith(searchText) || searchText.StartsWith(edge.Key))
                {
                    if (edge.Value.IsTerminal)
                    {
                        foreach (T w in edge.Value.Word)
                        {
                            result.Add(w);
                        }
                    }

                    if (edge.Key.Length <= searchText.Length)
                        SearchIntl(edge.Value, searchText[edge.Key.Length..], result);
                    else
                        CollectAllValues(edge.Value, result);
                }
            }
        }

        private static void SearchIntlExt<T>(
            SearchTrie<T>.Node node,
            string searchText,
            HashSet<T> result)
        {
            if (node.IsTerminal)
                return;

            foreach (var edge in node.Edges)
            {
                SearchIntl(
                    edge.Value,
                    searchText,
                    result);
                SearchIntlExt(
                    edge.Value,
                    searchText,
                    result);
            }
        }

        public static HashSet<T> Search<T>(
            this SearchTrie<T> items,
            string searchText)
        {
            searchText = searchText.ToLower();

            HashSet<T> result = new();

            SearchIntl(items.Root, searchText, result);
            SearchIntlExt(items.Root, searchText, result);
            return result;
        }
    }
}
