using Newtonsoft.Json;

namespace Geranium.ResourceManager
{
    public interface IResourceTrackProcessor
    {
        string ResourceFolder { get; }

        string ManifestFilePath { get; }

        void ProcessUpdateFile(string filePath);

        void ProcessNewFile(string filePath);

        void ProcessDeleteFile(string filePath);

        IEnumerable<string> FilterFiles(string[] trackedFilePaths);
    }

    public class ResourceInfo
    {
        public string Path { get; set; }

        public DateTime LastWriteTime { get; set; }
    }

    public class ResourceTrackerManifest
    {
        public List<ResourceInfo> Resources { get; set; } = new List<ResourceInfo>();
    }

    public class ResourceTracker
    {
        public ResourceTracker(IResourceTrackProcessor resourceTracker)
        {
            Processor = resourceTracker;
        }

        public IResourceTrackProcessor Processor { get; private set; }

        public ResourceTrackerManifest LastBuild { get; private set; }

        private List<string> ProcessedFilePath = new();

        public ResourceTrackerManifest CurrentBuild { get; private set; }

        public void Track()
        {
            LastBuild = GetLastResourceManifestBuild();
            CurrentBuild = new ResourceTrackerManifest();

            var manifestDirectory = Path.GetDirectoryName(Processor.ManifestFilePath);

            if (!Directory.Exists(manifestDirectory))
            {
                Directory.CreateDirectory(manifestDirectory);
            }

            TrackResources();

            WriteCurrentBuild();
        }

        private ResourceTrackerManifest GetLastResourceManifestBuild()
        {
            if (File.Exists(Processor.ManifestFilePath))
            {
                return JsonConvert.DeserializeObject<ResourceTrackerManifest>(File.ReadAllText(Processor.ManifestFilePath));
            }

            return new ResourceTrackerManifest();
        }

        private void TrackResources()
        {
            var trackFilePaths = Directory.GetFiles(Processor.ResourceFolder, "*.*", SearchOption.AllDirectories);

            var filteredFilePaths = Processor.FilterFiles(trackFilePaths);
            foreach (var filePath in filteredFilePaths)
            {
                ProcessFile(filePath);
            }

            var filePathsForDelete = LastBuild.Resources.Select(x => x.Path).Except(filteredFilePaths);
            foreach (var filePathForDelete in filePathsForDelete)
            {
                Processor.ProcessDeleteFile(filePathForDelete);
            }
        }

        private void ProcessFile(string filePath)
        {
            var lastTime = File.GetLastWriteTime(filePath);
            var res = LastBuild.Resources.FirstOrDefault(x => x.Path == filePath);

            CurrentBuild.Resources.Add(new ResourceInfo() { Path = filePath, LastWriteTime = lastTime });

            if (res == default)
            {
                Processor.ProcessNewFile(filePath);
            }
            else
            {
                if (res.LastWriteTime.ToString() != lastTime.ToString())
                {
                    Processor.ProcessUpdateFile(filePath);
                }
            }
        }

        private void WriteCurrentBuild()
        {
            var manifest = JsonConvert.SerializeObject(CurrentBuild, Formatting.Indented);

            File.WriteAllText(Processor.ManifestFilePath, manifest);
        }
    }
}
