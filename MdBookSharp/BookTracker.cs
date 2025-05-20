using Geranium.ResourceManager;
using MdBookSharp.Books;

namespace MdBookSharp
{
    internal class BookTracker : IResourceTrackProcessor
    {
        private Book book;

        public BookTracker(Book book)
        {
            this.book = book;
        }

        public string ResourceFolder => book.ProjectPath;

        public string ManifestFilePath => Path.Combine(book.ProjectBinPath,"increment.manifest");

        public IEnumerable<string> FilterFiles(string[] trackedFilePaths) 
            => trackedFilePaths.Where(x => Path.GetExtension(x) != ".md" && Path.GetFileName(x) != "settings.json");

        public void ProcessDeleteFile(string filePath)
        {
            var path = GetFileBinPath(filePath);
            File.Delete(Path.Combine(book.ProjectBinPath, path));
        }

        public void ProcessNewFile(string filePath) => ProcessNewOrUpdateFile(filePath);

        public void ProcessUpdateFile(string filePath) => ProcessNewOrUpdateFile(filePath);

        private void ProcessNewOrUpdateFile(string filePath)
        {
            var path = GetFileBinPath(filePath);

            path = Path.Combine(book.ProjectBinPath, path);

            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.Copy(filePath, path, true);
        }

        private string GetFileBinPath(string filePath)
        {
            var commonPath = filePath.Replace(book.ProjectPath, "");

            if (commonPath.StartsWith("theme"))
                commonPath = commonPath.Replace("theme\\", "");

            return commonPath;
        }
    }
}