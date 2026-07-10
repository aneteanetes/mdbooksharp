using mdbooksharplib.Books;

namespace mdbooksharplib.Extensions
{
    public class DiceExtension : MdBookExtension<DiceExtensionCfg>
    {
        public override void Process(Page file)
        {
            //not inversed for 'faster' check
            if (file.MdContent.Contains("d4") || file.MdContent.Contains("d6") || file.MdContent.Contains("d8") || file.MdContent.Contains("d10") || file.MdContent.Contains("d12") || file.MdContent.Contains("d20") || file.MdContent.Contains("d%"))
            {
                typeof(Dice).GetAllValues<Dice>().ForEach(d =>
                {
                    var diceToken = d.ToString();
                    if (d == Dice.dPercent)
                        diceToken = "d%";

                    var diceRender = DiceString(file, d, Settings.IconSize);
                    file.Html = file.Html.Replace(diceToken.ToString(), diceRender);
                });
            }
        }

        public static string DiceString(Page page, Dice dice, int size)
        {
            var diceImageName = dice.ToString();
            if (dice == Dice.dPercent)
                diceImageName = "d%";

            return $@"<img width=""{size}"" height=""{size}"" class=""dice"" src=""{page.PathToRoot}/images/dices/di{diceImageName.Substring(1)}.png"">";
        }
    }
}
