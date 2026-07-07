using Markdig;
using MdBookSharp.Resources;
using System.Diagnostics;

namespace MdBookSharp.Books
{
    [DebuggerDisplay("{ProjectRootPath}")]
    internal partial class Book
    {
        public string Title { get; set; }

        public string DefaultTheme { get; set; } = "rust";

        public bool IsFaviconSvg { get; set; }

        public bool IsPrintEnable { get; set; }

        public List<string> AdditionalCssFiles { get; set; } = new();

        public List<EmbeddedResource> ExtensionCssFiles { get; set; } = [];

        public List<string> AdditionalJsFiles { get; set; } = new();

        public bool IsFaviconPng { get; set; }

        public string FaviconExt { get; set; }

        public List<Page> Pages { get; set; } = new();

        public List<Menu> Menu { get; set; } = new();

        private List<Menu> _menuFlat { get; set; } = new();

        public void FlatMenuAdding(Menu menu) => _menuFlat.Add(menu);

        public IEnumerable<Menu> FlatMenu => _menuFlat;

        public List<Page> PagesHierarchy { get; set; } = new();

        public string ProjectPath { get; set; }

        public string ProjectRootPath { get; set; }

        public string Binpath { get; set; } = "bin";

        public string DevRootPath { get; set; }

        public Configuration Settings;

        public bool IsNeedGenerateNavBar { get; set; } = true;

        public string ProjectBinPath { get; internal set; }

        public MarkdownPipeline MarkdownPipeline { get; internal set; }


        public Dictionary<string, string> Manifest = new();
    }
}