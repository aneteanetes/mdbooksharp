using Geranium.Reflection;
using HtmlAgilityPack;
using Markdig;
using MdBookSharp.Books;
using System.Text.Json;

namespace MdBookSharp.Extensions.ChangelogExtensions
{
    internal class ChangelogExtension : MdBookExtension<ChangelogExtensionConfig>
    {
        private ChangeLogEntry[] _changeLog;
        private MarkdownPipeline _pipeline;

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
            _pipeline = pipeline;
            var logPath = Path.Combine(book.ProjectPath, this.Settings.ChangeLogPath);
            using var stream = File.OpenRead(logPath);
            _changeLog = JsonSerializer.Deserialize<ChangeLogEntry[]>(stream);

            var changePage = new Page()
            {
                Book = book,
                Prev = book.Pages.LastOrDefault(),
                Name = this.Settings.ChangeLogMenuName,
                IsCounted = false,
                Path = this.Settings.FullChangesUrl,
                Path_Html = this.Settings.FullChangesUrl,
                PathToRoot = Path.GetRelativePath(this.Settings.FullChangesUrl, "./").Replace("\\", "/").Substring(1),
                //Number = book.Pages.Count(p => p.IsCounted && p.Level==0).ToString(),
                MdContent = GenerateMdChangelog()
            };

            book.Pages.LastOrDefault()?.Next = changePage;
            book.Pages.Add(changePage);

            book.Menu.Add(new Menu(book) { Type = MenuType.Delimiter });
            book.Menu.Add(new Menu(book)
            {
                Page=changePage,
                Text = this.Settings.ChangeLogMenuName,
            });
        }

        private string GenerateMdChangelog()
        {
            string md = $"# {this.Settings.FullchangeText}";

            foreach (var entry in _changeLog.Reverse())
            {
                md += $@"
### {entry.When}
{entry.Text}

";
            }

            return md;
        }

        public override void Process(Page file)
        {
            if (file.MdContent.Contains("</changelog>"))
            {
                var logTag = file.HtmlDocument.DocumentNode.Descendants("changelog").FirstOrDefault();
                var inner = logTag.InnerHtml;

                var changelog = ChangeLogNode(file.HtmlDocument, inner);

                logTag.ParentNode.ReplaceChild(changelog, logTag);

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
            }
        }

        private HtmlNode ChangeLogNode(HtmlDocument doc, string innerHtml)
        {
            var node = doc.CreateElement("div");
            node.AddClass("layout-wrapper");

            var text = @$"
            <main class='changelog-main'>
                {innerHtml}
            </main>
            <aside class='updates-panel'>
                <h3>{this.Settings.ChangelogHeader}</h3>";

            for (int i = 1; i < this.Settings.EntryCount+1; i++)
            {
                var entry = this._changeLog[^i];
                var content = Markdown.ToHtml(entry.Text, _pipeline);
                text += $@"
                <div class='update-item'>
                    <span class='update-date'>{entry.When}</span>
                    {content}
                </div>";
            }

            text += @$"<a href='{this.Settings.FullChangesUrl}' class='all-updates'>{this.Settings.FullchangeText}</a>
                <div class='contacts-block' style='margin-top: 40px; border-top: 1px solid rgba(0,0,0,0.1); padding-top: 20px;'>
                    <p style='font-size: 0.9rem; opacity: 0.7;'>{this.Settings.ContactsHeader}</p>
                    <div style='display: flex; gap: 15px; font-size: 1.2rem;'>";

            if (this.Settings.TelegramLink.IsNotEmpty())
            {
                text += @$"<a href='{this.Settings.TelegramLink}' target='_blank' title='Telegram'><svg height='25' width='25' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 640 640'><path d='M320 72C183 72 72 183 72 320C72 457 183 568 320 568C457 568 568 457 568 320C568 183 457 72 320 72zM435 240.7C431.3 279.9 415.1 375.1 406.9 419C403.4 437.6 396.6 443.8 390 444.4C375.6 445.7 364.7 434.9 350.7 425.7C328.9 411.4 316.5 402.5 295.4 388.5C270.9 372.4 286.8 363.5 300.7 349C304.4 345.2 367.8 287.5 369 282.3C369.2 281.6 369.3 279.2 367.8 277.9C366.3 276.6 364.2 277.1 362.7 277.4C360.5 277.9 325.6 300.9 258.1 346.5C248.2 353.3 239.2 356.6 231.2 356.4C222.3 356.2 205.3 351.4 192.6 347.3C177.1 342.3 164.7 339.6 165.8 331C166.4 326.5 172.5 322 184.2 317.3C256.5 285.8 304.7 265 328.8 255C397.7 226.4 412 221.4 421.3 221.2C423.4 221.2 427.9 221.7 430.9 224.1C432.9 225.8 434.1 228.2 434.4 230.8C434.9 234 435 237.3 434.8 240.6z' /></svg></a>";
            }
            if (this.Settings.VKLink.IsNotEmpty())
            {
                text += @$"<a href='{this.Settings.VKLink}' target='_blank' title='VK'><svg height='25' width='25' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 640 640'><path d='M127.5 127.5C96 159 96 209.7 96 311L96 329C96 430.3 96 481 127.5 512.5C159 544 209.7 544 311 544L328.9 544C430.3 544 481 544 512.4 512.5C543.8 481 544 430.3 544 329L544 311.1C544 209.7 544 159 512.5 127.6C481 96.2 430.3 96 329 96L311 96C209.7 96 159 96 127.5 127.5zM171.6 232.3L222.7 232.3C224.4 317.8 262.1 354 292 361.5L292 232.3L340.2 232.3L340.2 306C369.7 302.8 400.7 269.2 411.1 232.3L459.3 232.3C455.4 251.5 447.5 269.6 436.2 285.6C424.9 301.6 410.5 315.1 393.7 325.2C412.4 334.5 428.9 347.6 442.1 363.7C455.3 379.8 465 398.6 470.4 418.7L417.4 418.7C412.5 401.2 402.6 385.6 388.8 373.7C375 361.8 358.1 354.3 340.1 352.1L340.1 418.7L334.3 418.7C232.2 418.7 174 348.7 171.5 232.2z' /></svg></a>";
            }

            text += $@"</div>
                </div>
            </aside>";

            node.InnerHtml = text;

            return node;
        }
    }
}
