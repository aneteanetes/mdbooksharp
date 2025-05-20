namespace MdBookSharp.Extensions.ImageToken
{
    internal class ImageTokenExtensionSettings
    {
        public List<string> Tokens = [];

        public Dictionary<string, string> TokenMappings = [];

        public int IconSize { get; set; } = 24;
    }
}
