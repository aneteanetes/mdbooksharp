using Geranium.Reflection;
using MdBookSharp.Books;

namespace MdBookSharp
{
    internal partial class BookParser
    {
        public static Book Parse(string path)
        {
            ConsoleLog.WriteLine("Parsing summary.md...");

            var summaryPath = Path.Combine(path, "SUMMARY.md");

            var content = File.ReadAllLines(summaryPath);
            var book = new Book();
            book.Title = content[0].Replace("#", "").Trim();

            Page prev = null;
            Page prevCounted = null;

            int rootCounter = 1;
            int counter = 0;
            List<string> prefixes = new();
            int currentLevel = 0;

            Dictionary<int, int> levelCounters = new();
            Stack<Page> levels = new Stack<Page>();

            foreach (var entry in content.Skip(1).ToArray())
            {
                if (entry.IsEmpty())
                    continue;

                var comment = entry.IndexOf("//");
                var line = comment > 0
                    ? entry.Substring(0, entry.IndexOf("//"))
                    : entry;

                Page page = new()
                {
                    Book = book
                };

                if (prev != null)
                    page.Prev = prev;

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

                bool urlExists = false;

                page.IsCounted = line.Contains("-");
                page.IsCollapsible = line.Contains("+");

                from = line.IndexOf("(") + 1;
                if (from > 0)
                {
                    to = line.IndexOf(")") - from;

                    //////////////path
                    urlExists = true;

                    page.Path = line.Substring(from, to);
                    page.Path_Html = page.Path.Replace(".md", ".html");
                    page.PathToRoot = Path.GetRelativePath(page.Path_Html, "./")
                        .Replace("\\", "/");
                    page.PathToRoot = page.PathToRoot.Substring(1);
                }
                else
                {
                    //page.IsCounted = false;
                }

                var targetFrom = line.IndexOf("{");
                if (targetFrom > 0)
                {
                    var targetTo = line.IndexOf("}") - from;
                    page.Target = line.Substring(from, to);
                }

                var layer = page.Level = line.TakeWhile(c => c == ' ').Count() / 2; 

                if (layer > currentLevel)
                {
                    levels.Push(prevCounted);
                    if (page.IsCounted)
                    {
                        prefixes.Add(prevCounted.Number);
                        currentLevel = layer;
                        levelCounters[layer] = counter;
                    }
                }
                else if (layer < currentLevel)
                {
                    if (levels.Count>0)
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

                if (prev != null && urlExists)
                {
                    prev.Next = page;
                }

                if (urlExists)
                {
                    prev = page;
                    prevCounted = page;
                    page.PathPhysical = page.Path.Replace("./", path);
                    page.MdContent = File.ReadAllText(page.PathPhysical);
                }
                else
                {
                    prevCounted = page;
                }

                book.Pages.Add(page);
            }

            book.PagesHierarchy = book.Pages.Where(x=>x.Parent is null).ToList();

            return book;
        }
    }
}