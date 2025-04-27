namespace MdBookSharp.Extensions.ImageToken
{
    internal class ImageTokenExtensionSettings
    {
        public Dictionary<string, string> TokenMappings = new()
        {
            {"СИЛ","str" },
            {"ЛВК","agi" },
            {"ИНТ","int" },
            {"ВЫН","end" },
            {"ДУХ","spr" },
            {"ХАР","cha" },
        };

        public int IconSize { get; set; } = 24;
    }
}
