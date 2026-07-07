using Markdig;
using MdBookSharp.Books;
using MoonSharp.Interpreter;
using System.Text.RegularExpressions;

namespace MdBookSharp.Extensions.LuaScriptExtension
{
    internal partial class LuaExtension : MdBookExtension<LuaExtensionConfig>
    {
        public override bool IsStaticHtmlApplicable => true;

        Script script;

        private bool isInited = false;

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
            if (isInited)
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
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            isInited = true;
        }

        public override void Process(Page file) { }

        public override string ProcessMd(Page file, string md)
        {
            var regex = VariableRegexp();
            string result = regex.Replace(md, match => {
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

        public string ProcessString(string text)
        {
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
        {
            var staticRegexp = VariableRegexp();
            var processed = staticRegexp.Replace(content, match =>
            {
                var code = match.Value.Replace("//%", "");
                try
                {
                    var val = script.DoString($"return {code}");
                    var str = val.CastToString();
                    return str;
                }
                catch (Exception ex)
                {
                    ConsoleLog.Error(code);
                    throw;
                }
            });
            return processed;
        }

        [GeneratedRegex(@"//%(?<value>\S*?(\((?>[^()]+|\((?<D>)|\)(?<-D>))*(?(D)(?!))\)|(?=\s|$)))")]
        private static partial Regex VariableRegexp();
    }
}
