using I18Next.Net;
using I18Next.Net.Backends;
using I18Next.Net.Plugins;
using MdBookSharp;
using MdBookSharp.Extensions;
using MdBookSharp.Extensions.ChangelogExtensions;
using MdBookSharp.Extensions.Dices;
using MdBookSharp.Extensions.ImageToken;
using MdBookSharp.Extensions.LuaScriptExtension;
using MdBookSharp.Extensions.MDLinkToHtmlExtension;
using MdBookSharp.Extensions.NabPlateExtension;
using MdBookSharp.Extensions.NavbarImage;
using MdBookSharp.Extensions.Searching;
using MdBookSharp.Extensions.WowIcons;
using MdBookSharp.Extensions.WoWPlates;
using MdBookSharp.Extensions.WrappedTable;
using MdBookSharp.Localization;
using System.Diagnostics;

ConsoleLog.WriteLine("Loading book...");

var luaExtension = new LuaExtension();

List<MdBookExtension> extensions = [
    new ChangelogExtension(),
    luaExtension,
    new SearchExtension(),
    new WoWPlateExtension(),
    new WrappedTableExtension(),
    new WoWIconExtension(),
    new DiceExtension(),
    new ImageTokenExtension(),
    new MDLinkToHtmlExtension(),
    new NavbarImageExtension(),
    new NabPlateExtension(),
    new FragmentsExtension(),
];

bool isDebug = false;
if (args.ElementAtOrDefault(1) != default)
{
    if(!bool.TryParse(args[1], out isDebug))
    {
        isDebug = false;
    }
}

var backend = new EmbeddedJsonFileBackend();
var translator = new DefaultTranslator(backend);
var i18n = new I18NextNet(backend, translator)
{
    Language = "ru" // default lang
};

var book = BookLoader.Load(args[0], isDebug, luaExtension, extensions);
i18n.Language = book.Settings.Language;
BookRenderer.Render(book, extensions,i18n);
BookBuilder.Build(book, extensions);

double elapsedMs = Stopwatch.GetElapsedTime(ConsoleLog.started, Stopwatch.GetTimestamp()).TotalMilliseconds;
Console.WriteLine($"Done... [{elapsedMs} ms]");