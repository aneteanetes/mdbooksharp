using MdBookSharp.Books;

namespace MdBookSharp.Extensions.Dices
{
    internal class DiceExtension : MdBookExtension<DiceExtensionCfg>
    {
        public override void Process(Page file)
        {
            //not inversed for 'faster' check
            if (file.MdContent.Contains("d4") || file.MdContent.Contains("d6") || file.MdContent.Contains("d8") || file.MdContent.Contains("d10") || file.MdContent.Contains("d12") || file.MdContent.Contains("d20") || file.MdContent.Contains("d%"))
            {
                typeof(Dice).GetAllValues<Dice>().ForEach(d =>
                {
                    var diceToken = d.ToString();
                    if (d == Dices.Dice.dPercent)
                        diceToken = "d%";

                    var diceRender = Dice(file, d, Settings.IconSize);
                    file.Html = file.Html.Replace(diceToken.ToString(), diceRender);
                });
            }
        }

        public static string Dice(Page page, Dice dice, int size)
        {
            var diceImageName = dice.ToString();
            if (dice == Dices.Dice.dPercent)
                diceImageName = "d%";

            return $@"<img width=""{size}"" height=""{size}"" class=""dice"" src=""{page.PathToRoot}/images/dices/di{diceImageName.Substring(1)}.png"">";
        }
    }
}
