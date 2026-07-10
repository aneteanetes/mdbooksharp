using Geranium.Reflection;
using Markdig;
using mdbooksharplib.Books;
using MoonSharp.Interpreter;
using System.Text.RegularExpressions;

namespace mdbooksharplib.Extensions
{
    public partial class LuaExtension : MdBookExtension<LuaExtensionConfig>
    {
        public override bool IsStaticHtmlApplicable => true;

        Script script;

        private bool isInited = false;

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
            if (isInited || this.Settings.ScriptsFolder.IsEmpty())
                return;

            script = new Script();
            var path = Path.Combine(book.ProjectPath, this.Settings.ScriptsFolder);
            var files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    script.DoFile(file);
                }
                catch (Exception ex)
                {
                    ConsoleLog.Error(ex.ToString());
                }
            }

            isInited = true;
        }

        public override void Process(Page file) { }

        public override string ProcessMd(Page file, string md)
            => ProcessString(md);

        public string ProcessString(string text)
        {
            if (!isInited)
                return text;

            var regex = VariableRegexp();
            string result = regex.Replace(text, match => {
                string key = match.Value.Replace("//%", "");
                try
                {
                    var val = script.DoString($"return {key}");
                    var str = val.CastToString();
                    return str;
                }
                catch (Exception ex)
                {
                    ConsoleLog.Error(key);
                    throw;
                }
            });

            return result;
        }

        public override string ProcessStaticHtml(string content)
            => ProcessString(content);

        [GeneratedRegex(@"//%(?<value>\S*?(\((?>[^()]+|\((?<D>)|\)(?<-D>))*(?(D)(?!))\)|(?=\s|$)))")]
        private static partial Regex VariableRegexp();
    }
}
