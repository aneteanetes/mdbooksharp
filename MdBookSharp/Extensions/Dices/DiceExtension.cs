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

                    var diceRender = Dice(file, d);
                    file.Html = file.Html.Replace(diceToken.ToString(), diceRender);
                });
            }
        }

        private string Dice(Page page, Dice dice)
        {
            var diceImageName = dice.ToString();
            if (dice == Dices.Dice.dPercent)
                diceImageName = "d%";

            var size = Settings.IconSize;
            return $@"<img width=""{size}"" height=""{size}"" class=""dice"" src=""{page.PathToRoot}images/dices/{diceImageName}.png"" alt=""{diceImageName}"">";
        }
    }
}
