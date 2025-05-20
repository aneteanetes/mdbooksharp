using MdBookSharp.Resources;

namespace MdBookSharp.Books
{
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

        public string Language { get; set; } = "ru";

        public List<Page> Pages { get; set; } = new();

        public List<Page> PagesHierarchy { get; set; } = new();

        public string RenderedNavbar { get; set; }

        public string ProjectPath { get; set; }

        public string ProjectRootPath { get; set; }

        public string Binpath { get; set; } = "bin";

        public string DevRootPath { get; set; }

        public Configuration Configuration;

        public bool IsNeedGenerateNavBar { get; set; } = true;
        public string ProjectBinPath { get; internal set; }

        public Dictionary<string, string> Manifest = new();
    }
}