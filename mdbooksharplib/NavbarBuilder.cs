using Geranium.Reflection;
using Markdig.Syntax;
using mdbooksharplib.Books;

namespace mdbooksharplib
{
    internal class NavbarBuilder
    {
        public Book Book { get; }

        public string NavBarTemplate { get; private set; }

        public string NavBarCollapibleStyles { get; private set; }

        public bool IsRendered { get; private set; }

        public NavbarBuilder(Book book)
        {
            Book = book;
            //NavbarRender();
        }

        public NavbarBuilder Render()
        {
            foreach (var menu in Book.Menu)
            {
                RenderMenu(menu);
            }

            this.IsRendered = true;
            return this;
        }

        private void RenderMenu(Menu menu)
        {
            NavBarTemplate += menu.Type == MenuType.Delimiter
                ? "<div class='spacer'></div>"
                : MenuItemTemplate(menu);

            var collapsibleParents = menu.GetAllParentCollapsible();
            foreach (var collapsible in collapsibleParents)
            {
                NavBarCollapibleStyles += $".chapter:has([data-path='{collapsible.DataPath}'].collapsed) [data-path='{menu.DataPath}'] {{ display: none !important; }}"+Environment.NewLine;
            }

            foreach (var child in menu.Children)
            {
                RenderMenu(child);
            }
        }

        private string MenuItemTemplate(Menu item)
        {
            return $"""
                <div class="chapter-item {(item.Type == MenuType.Collapsible ? "collapsible collapsed" : "")}" data-path="{item.DataPath}" data-level="{item.Level}">
                    <{(item.Page != null ? $"a href='{{{item.Page.Id}Url}}'" : "span")}>{(item.Number.IsNotEmpty() ? $"<strong>{item.Number}.</strong>" : "")}{item.Text}</{(item.Page !=null ? "a" : "span")}>
                    {(item.Type == MenuType.Collapsible ? "<span class='toggle'><i class='fa fa-angle-right'></i></span>" : "")}
                    {item.Id}Submenu
                    </div>
                """;
        }

        private string Submenu(Menu item)
        {
            if (!item.IsSidebarExpands)
                return string.Empty;

            var headers = item.Page.MdDocument.Descendants<HeadingBlock>();

            var subsidebar = "<div class='page-headers-tree'>";

            var i = 0;

            foreach (HeadingBlock header in headers)
            {
                int level = header.Level;
                if (level == 0)
                    continue;

                string text = header.Inline?.FirstChild?.ToString() ?? "";
                var link = text.Replace(" ", "-");

                subsidebar += $"<a href='#{link}' class='header-link {(i==0 ? "active":"")}' data-header-level='{level}'>{text}</a>" + Environment.NewLine;

                i++;
            }

            return subsidebar + "</div>";
        }

        public string Build(Page renderingPage)
        {
            var result = "<style>" + NavBarCollapibleStyles + "</style>" +
                NavBarTemplate.Replace("{" + renderingPage.Id + "Url}'", "{" + renderingPage.Id + "Url}'" + @" class=""active""")
                .Replace(renderingPage.Menu.Id+"Submenu", Submenu(renderingPage.Menu));

            if(renderingPage.Menu.Type== MenuType.Collapsible)
            {
                result = result.Replace($"collapsed\" data-path=\"{renderingPage.Menu.DataPath}\"", $"\" data-path=\"{renderingPage.Menu.DataPath}\"");
            }

            var collapsibleParents = renderingPage.Menu.GetAllParentCollapsible();
            foreach (var collapsibleParent in collapsibleParents)
            {
                result = result.Replace($"collapsed\" data-path=\"{collapsibleParent.DataPath}\"", $"\" data-path=\"{collapsibleParent.DataPath}\"");
            }

            foreach (var page in Book.Pages)
            {
                var url = GetRelativePath(renderingPage, page);

                result = result.Replace("{" + page.Id + "Url}", url);
            }

            foreach (var menu in Book.Menu)
            {
                result = ClearMenu(result, menu);
            }

            return result;
        }

        private string ClearMenu(string result, Menu menu)
        {
            result = result.Replace(menu.Id + "Submenu", "");

            foreach (var submenu in menu.Children)
            {
                result = ClearMenu(result, submenu);
            }

            return result;
        }

        public static string GetRelativePath(Page relativeToPage, Page pagePath)
        {
            if (relativeToPage == null || pagePath == null || relativeToPage == pagePath || relativeToPage.Path == null || pagePath.Path == null)
                return null;

            var pathRel = Path.GetRelativePath(relativeToPage.Path_Html, pagePath.Path_Html)
                    .Replace("\\", "/");

            if(pathRel.Length>1)
                    return pathRel.Substring(3);

            if (pathRel == ".")
                return "./index.html";

            return pathRel;
        }
    }
}