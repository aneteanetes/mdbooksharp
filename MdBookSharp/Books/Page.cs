using Geranium.Reflection;
using HtmlAgilityPack;
using Markdig.Syntax;
using MdBookSharp.Resources;
using System.Diagnostics;

namespace MdBookSharp.Books
{
    [DebuggerDisplay("{DEBUGVIEW}")]
    internal class Page
    {
        private string DEBUGVIEW => Name == null ? "Delimiter" : $@"{Name} {Path}";

        public string IsActive { get; set; }

        public string Name { get; set; }

        public string FileNameWithoutExtension
        {
            get
            {
                if (fileNameWithoutExtension == null)
                    fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Path);

                return fileNameWithoutExtension;
            }
        }
        private string fileNameWithoutExtension;

        public string PathToRoot { get; set; }

        public string Path { get; set; }

        public string Path_Html { get; set; }

        public string PathPhysical { get; set; }

        public string[] ExtensionCssFiles { get; set; } = [];

        public string Url { get; set; }

        public bool IsCounted { get; set; } = true;

        public bool IsCollapsible { get; set; } = true;

        public string Number { get; set; }

        public string CounterNumber { get; set; }

        public string MdContent { get; set; }

        public string Content { get; set; }

        public string Html { get; set; }

        public string InnerText { get; set; }

        public Page Prev { get; set; }

        public Page Next { get; set; }

        public int Level { get; set; }

        public int LevelFolder { get; set; }

        public bool IsRendered { get; set; }

        public bool IsPrevExists => Prev != null;

        public bool IsNextExists => Next != null;

        public HtmlDocument HtmlDocument { get; internal set; }

        public Page Parent { get; set; }
        public bool IsDelimiter { get; internal set; }

        public Book Book { get; set; }
    }
}
