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

        public override void Init(Book book, MarkdownPipeline pipeline)
        {
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
                catch
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
                catch
                {
                    ConsoleLog.Error(code);
                    throw;
                } 
            });
            return processed;
        }

        [GeneratedRegex(@"//%[\w.]+(\(\))?")]
        private static partial Regex VariableRegexp();

        [GeneratedRegex(@"//%.*")]
        private static partial Regex StaticLuaRegexp();
    }
}
