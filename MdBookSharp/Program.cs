using MdBookSharp.Books;
using MdBookSharp.Extensions;
using MdBookSharp.Extensions.Dices;
using MdBookSharp.Extensions.ImageToken;
using MdBookSharp.Extensions.MDLinkToHtmlExtension;
using MdBookSharp.Extensions.Searching;
using MdBookSharp.Extensions.WoWPlates;

List<MdBookExtension> extensions = [
    new SearchExtension(),
    new DiceExtension(),
    new ImageTokenExtension(),
    new MDLinkToHtmlExtension(),
    new WoWPlateExtension(),
];

Book.Load(args[0])
    .Render(extensions)
    .Build();