namespace MdBookSharp.Extensions.ChangelogExtensions
{
    public class ChangelogExtensionConfig
    {
        public string ChangeLogPath { get; set; }
        public string ChangelogHeader { get;  set; }
        public string FullchangeText { get;  set; }
        public string ContactsHeader { get;  set; }
        public string TelegramLink { get;  set; }
        public string VKLink { get;  set; }
        public string FullChangesUrl { get;  set; }
        public int EntryCount { get;  set; }
        public string ChangeLogMenuName { get; set; }
    }

    internal class ChangeLogEntry
    {
        public string When { get; set; }

        public string Text { get; set; }
    }
}
