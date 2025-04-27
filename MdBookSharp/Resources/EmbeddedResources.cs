using System.Diagnostics;
using System.Reflection;

namespace MdBookSharp.Resources
{
    internal class EmbeddedResources
    {
        private static Assembly Assembly = Assembly.GetExecutingAssembly();

        private static Dictionary<string, string> Cache = new();

        private static string ResourceRoot = $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.";

        public static string GetEmbeddedFileContent(string relativeFilePath)
        {
            if(Cache.ContainsKey(relativeFilePath)) 
                return Cache[relativeFilePath];

            var res = relativeFilePath.Contains(ResourceRoot)
                ? relativeFilePath
                : $"MdBookSharp.Resources.{Embedded(relativeFilePath)}";

            MemoryStream ms = new MemoryStream();
            using (Stream stream = Assembly.GetManifestResourceStream(res))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                stream.CopyTo(ms);
            }

            if (ms.CanSeek)
            {
                ms.Seek(0, SeekOrigin.Begin);
            }

            using StreamReader reader = new StreamReader(ms);
            return Cache[relativeFilePath] = reader.ReadToEnd();
        }

        public static MemoryStream GetEmbeddedFile(string relativeFilePath)
        {
            var res = relativeFilePath.Contains(ResourceRoot)
                ? relativeFilePath
                : $"MdBookSharp.Resources.{Embedded(relativeFilePath)}";

            MemoryStream ms = new MemoryStream();
            using (Stream stream = Assembly.GetManifestResourceStream(res))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                stream.CopyTo(ms);
            }

            if (ms.CanSeek)
            {
                ms.Seek(0, SeekOrigin.Begin);
            }

            return ms;
        }

        public static List<EmbeddedResource> GetEmbeddedFolder(string path)
        {
            List<EmbeddedResource> Files = new();

            var names = Assembly.GetManifestResourceNames();
            var inFolder = names.Where(n=>n.Contains(path)).ToArray();
            foreach (var filePath in inFolder)
            {
                var splitted = filePath.Split('.');
                var fileName = string.Join(".", splitted.TakeLast(2));

                var name = filePath.Replace(fileName, "")
                    .Replace(ResourceRoot,"")
                    .Replace(path+".","")
                    .Replace(".", "\\");

                Files.Add(new EmbeddedResource()
                {
                    FileName = Path.Combine(name,fileName),
                    Content = GetEmbeddedFile(filePath),
                    Path = filePath
                });
            }

            return Files;
        }

        private static string Embedded(string path) => path.Replace(@"\", ".").Replace(@"/", ".");
    }

    [DebuggerDisplay("{FileName}")]
    internal class EmbeddedResource
    {
        public string FileName { get; set; }

        public string Path { get; set; }

        public MemoryStream Content { get; set; }
    }
}
