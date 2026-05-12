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
using System.Diagnostics;

ConsoleLog.WriteLine("Loading book...");

List<MdBookExtension> extensions = [
    new ChangelogExtension(),
    new LuaExtension(),
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

var book = BookLoader.Load(args[0], isDebug);
BookRenderer.Render(book, extensions);
BookBuilder.Build(book, extensions);

double elapsedMs = Stopwatch.GetElapsedTime(ConsoleLog.started, Stopwatch.GetTimestamp()).TotalMilliseconds;
Console.WriteLine($"Done... [{elapsedMs} ms]");