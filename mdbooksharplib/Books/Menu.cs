using Geranium.Reflection;
using System.Diagnostics;

namespace mdbooksharplib.Books
{
    [DebuggerDisplay("{DEBUGVIEW}")]
    public class Menu
    {
        public Menu(Book book)
        {
            book.FlatMenuAdding(this);
            Id = Guid.NewGuid().ToString().Split('-')[book.Random.Next(0, 5)];
        }

        private string DEBUGVIEW
        {
            get
            {
                string type = "";
                switch (Type)
                {
                    case MenuType.Static:
                        type = "*";
                        break;
                    case MenuType.Collapsible:
                        type = "+";
                        break;
                    case MenuType.Delimiter:
                        return "-";
                    default:
                        break;
                }

                return $"{type} {Number} {(Page == null ? Text : Page.Name)} {(_children.Count > 0 ? "(+)" : "")}";
            }
        }

        public string Id { get; }

        public string DataPath => Id;

        public string Text { get; set; }

        public MenuType Type { get; set; }

        private Page _page;
        public Page Page
        {
            get => _page;
            set
            {
                _page = value;
                _page.Menu = this;
            }
        }

        public string Number { get; set; }

        public int Level { get; set; }

        private List<Menu> _children = new();

        public IEnumerable<Menu> Children => _children;

        public void AddChild(Menu menu)
        {
            _children.Add(menu);
            menu.Parent = this;
        }

        public Menu Parent { get; set; }

        public string GetNextChildNumber()
        {
            if (Number.IsEmpty())
                return $"{_children.Count + 1}";
            else
                return $"{Number}.{_children.Count + 1}";
        }

        public List<Menu> GetAllParentCollapsible()
        {
            if (Parent != null)
            {
                if (Parent.Type == MenuType.Collapsible)
                    return [Parent, ..Parent.GetAllParentCollapsible()];

                return Parent.GetAllParentCollapsible();
            }

            return [];
        }

        public Menu GetFirstCollapsible()
        {
            if (Parent == null)
                return default;

            if (Parent.Type == MenuType.Collapsible)
                return Parent;

            return Parent.GetFirstCollapsible();
        }

    }
}