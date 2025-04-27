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


var path = "C:\\Users\\anete\\source\\repos\\wow2drusadapt";/* Path.GetFullPath("..\\Book\\");
var asmname = Assembly.GetEntryAssembly().GetName().Name;

void FindBook()
{
    var dir = Directory.GetParent(path);
    path = dir.FullName;
    if (dir.Name != asmname)
        FindBook();
}
FindBook();

path = Path.Combine(Directory.GetParent(path).FullName, "Book");
*/

Book.Load(path)
    .Render(extensions)
    .Build();