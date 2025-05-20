using MdBookSharp;
using MdBookSharp.Books;
using MdBookSharp.Extensions;
using MdBookSharp.Extensions.Dices;
using MdBookSharp.Extensions.ImageToken;
using MdBookSharp.Extensions.MDLinkToHtmlExtension;
using MdBookSharp.Extensions.Searching;
using MdBookSharp.Extensions.WowIcons;
using MdBookSharp.Extensions.WoWPlates;
using MdBookSharp.Extensions.WrappedTable;

List<MdBookExtension> extensions = [
    new SearchExtension(),
    new WoWPlateExtension(),
    new WrappedTableExtension(),
    new WoWIconExtension(),
    new DiceExtension(),
    new ImageTokenExtension(),
    new MDLinkToHtmlExtension()
];

Console.WriteLine("Loading book...");
var book = BookLoader.Load(args[0]);
BookRenderer.Render(book, extensions);
BookBuilder.Build(book);
