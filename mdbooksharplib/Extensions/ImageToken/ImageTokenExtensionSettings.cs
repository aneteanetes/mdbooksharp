namespace mdbooksharplib.Extensions
{
    public class ImageTokenExtensionSettings
    {
        public List<string> Tokens { get; set; } = [];

        public Dictionary<string, string> TokenMappings { get; set; } = [];

        public int IconSize { get; set; } = 24;
    }
}
