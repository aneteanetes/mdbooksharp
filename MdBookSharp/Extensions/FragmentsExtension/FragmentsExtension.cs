using HtmlAgilityPack;
using Markdig;
using MdBookSharp.Books;

namespace MdBookSharp.Extensions
{
    internal class FragmentsExtension : MdBookExtension<FragmentsExtensionConfig>
    {
        public override bool IsGlobal => true;

        private readonly List<string> fragments = new();

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
            foreach (var file in Directory.GetFiles(Path.Combine(book.ProjectPath,"theme"), "*.html", SearchOption.AllDirectories))
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
