using Geranium.Reflection;
using mdbooksharplib.Extensions;
using mdbooksharplib.Resources;
using System.Diagnostics;

namespace mdbooksharplib
{
    public static class BookWizard
    {
        private static string asmPath;
        static BookWizard()
        {
            asmPath = AppContext.BaseDirectory;
        }

        /// <summary>
        /// Generate or init book based on existance of summary.md file
        /// </summary>
        /// <param name="isLog"></param>
        public static void GenerateOrInit(bool isLog = true)
        {
            var path = Path.Combine(asmPath, "book");
            if (IsBookExists(path))
            {
                Generate(path);
            }
            else
            {
                Init(path);
            }
        }

        /// <summary>
        /// Generate book with default extension list
        /// </summary>
        /// <param name="bookPath"></param>
        /// <param name="isLog"></param>
        public static void Generate(string bookPath, bool isLog = true)
            => Generate(bookPath,
            [
                new ChangelogExtension(),
                new SearchExtension(),
                new WoWPlateExtension(),
                new WrappedTableExtension(),
                new WoWIconExtension(),
                new DiceExtension(),
                new ImageTokenExtension(),
                new MDLinkToHtmlExtension(),
                new NavbarImageExtension(),
                new NabPlateExtension(),
                new FragmentsExtension(),
            ], isLog);

        /// <summary>
        /// Generate book with specific extension list (lua automatically included)
        /// </summary>
        /// <param name="bookPath"></param>
        /// <param name="extensions"></param>
        /// <param name="isLog"></param>
        public static void Generate(string bookPath, List<MdBookExtension> extensions, bool isLog=true)
        {
            if(bookPath.IsEmpty())
                bookPath = AppContext.BaseDirectory;

            ConsoleLog.IsLog = isLog;
            ConsoleLog.WriteLine("Loading book...");

            var luaExtension = new LuaExtension();

            extensions = [luaExtension, ..extensions];

            var book = BookLoader.Load(bookPath, false, luaExtension, extensions);
            BookRenderer.Render(book, extensions);
            BookBuilder.Build(book, extensions);

            double elapsedMs = Stopwatch.GetElapsedTime(ConsoleLog.started, Stopwatch.GetTimestamp()).TotalMilliseconds;
            Console.WriteLine($"Done... [{elapsedMs} ms]");
        }

        /// <summary>
        /// Init new book template inside directory path
        /// </summary>
        /// <param name="bookPath"></param>
        /// <param name="isLog"></param>
        public static void Init(string bookPath, bool isLog = true)
        {
            if (bookPath.IsEmpty())
                bookPath = asmPath;

            ConsoleLog.IsLog = isLog;

            ConsoleLog.WriteLine("Checking book exists...");

            if (IsBookExists(bookPath))
            {
                ConsoleLog.Error("Book already exists...");
            }

            ConsoleLog.WriteLine("Initializing new book...");

            var templateFiles = EmbeddedResources.GetEmbeddedFolder("template");

            var srcPath = bookPath;
            if(!Directory.Exists(srcPath))
                Directory.CreateDirectory(srcPath);

            foreach (var templateFile in templateFiles)
            {
                var filePath = Path.Combine(srcPath,templateFile.FileName);

                FileInfo fileInfo = new FileInfo(filePath);

                fileInfo.Directory?.Create();

                var content = templateFile.Content.ToArray();

                using (var sw = fileInfo.Create())
                {
                    sw.Write(content,0, content.Length);
                }
            }

            double elapsedMs = Stopwatch.GetElapsedTime(ConsoleLog.started, Stopwatch.GetTimestamp()).TotalMilliseconds;
            Console.WriteLine($"Done... [{elapsedMs} ms]");
        }

        /// <summary>
        /// Check book exists by summary.md file
        /// </summary>
        /// <param name="bookPath"></param>
        /// <returns></returns>
        public static bool IsBookExists(string bookPath)
        {
            var summaryPath = Path.Combine(bookPath, "src", "SUMMARY.md");
            return File.Exists(summaryPath);
        }
    }
}
