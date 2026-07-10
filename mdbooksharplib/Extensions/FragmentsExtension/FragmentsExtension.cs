using HtmlAgilityPack;
using Markdig;
using mdbooksharplib.Books;

namespace mdbooksharplib.Extensions
{
    public class FragmentsExtension : MdBookExtension<FragmentsExtensionConfig>
    {
        public override bool IsGlobal => true;

        private readonly List<string> fragments = new();

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
            var path = Path.Combine(book.ProjectPath, "theme");
            if (!Directory.Exists(path))
                return;

            foreach (var file in Directory.GetFiles(path, "*.html", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);
                fragments.Add(content);
            }
        }

        public override void Process(Page file)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(file.Content);

            var body = htmlDoc.DocumentNode.Descendants("body").FirstOrDefault();

            foreach (var fragment in fragments)
            {
                var processedFragment = fragment.Replace("&PathToRoot", file.PathToRoot);
                body.AppendChild(HtmlNode.CreateNode(processedFragment));
            }

            file.Content = htmlDoc.DocumentNode.InnerHtml;
        }
    }
}
