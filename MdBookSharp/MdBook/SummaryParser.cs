using Geranium.Reflection;
using MdBookSharp.Books;
using System.Text.RegularExpressions;

namespace MdBookSharp.MdBook
{
    internal class SummaryParser
    {
        public static Book Parse(string path)
        {
            var content = File.ReadAllLines(path);
            var book = new Book();
            book.Title = content[0].Replace("#", "").Trim();

            Page prev = null;

            int rootCounter = 1;
            int counter = 0;
            List<string> prefixes = new();
            int currentLevel = 0;

            Dictionary<int, int> levelCounters = new();
            Stack<Page> levels = new Stack<Page>();

            foreach (var line in content.Skip(1).ToArray())
            {
                if (line.IsEmpty())
                    continue;

                Page page = new();

                Console.WriteLine($"Parsing {Array.IndexOf(content, line)} page of {content.Length - 1}...");

                if (line.Contains("#"))
                {
                    page.Name = line.Replace("#", "").Trim();
                    book.Pages.Add(page);
                    continue;
                }

                if (line == "---")
                {
                    page.IsDelimiter = true;
                    book.Pages.Add(page);
                    continue;
                }

                var from = line.IndexOf("[") + 1;
                var to = line.IndexOf("]") - from;
                page.Name = line.Substring(from, to);

                from = line.IndexOf("(") + 1;
                if (from > 0)
                {
                    to = line.IndexOf(")") - from;
                    page.Path = line.Substring(from, to);
                }

                page.IsCounted = line.Contains("-");
                page.IsCollapsible = line.Contains("+");

                var layer = page.Level = Regex.Matches(line, "   ").Count;


                if (layer > currentLevel)
                {
                    levels.Push(prev);
                    if (page.IsCounted)
                    {
                        prefixes.Add(prev.Number);
                        currentLevel = layer;
                        levelCounters[layer] = counter;
                    }
                }
                else if (layer < currentLevel)
                {
                    levels.Pop();
                    if (page.IsCounted)
                    {
                        prefixes.RemoveAt(prefixes.Count - 1);
                        currentLevel = layer;
                    }
                }


                if (layer == 0)
                {
                    if (page.IsCounted)
                    {
                        page.Number = $"{rootCounter}";
                        rootCounter++;
                        levelCounters.Clear();
                    }

                    levels.Clear();
                }
                else
                {

                    if (page.IsCounted)
                    {
                        levelCounters[layer]++;
                        page.Number = prefixes.Last() + $".{levelCounters[layer]}";
                    }
                }

                if (levels.Count > 0)
                    page.Parent = levels.Peek();

                prev = page;
                book.Pages.Add(page);
            }

            book.PagesHierarchy = book.Pages.Where(x=>x.Parent is null).ToList();

            return book;
        }
    }
}