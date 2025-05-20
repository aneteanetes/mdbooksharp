using Geranium.Reflection;
using HtmlAgilityPack;
using MdBookSharp.Books;
using MdBookSharp.Extensions.Dices;
using MdBookSharp.Extensions.ImageToken;
using System.Text.RegularExpressions;

namespace MdBookSharp.Extensions.WoWPlates
{
    internal class WoWPlateExtension : MdBookExtension<WowPlateExtensionConfig>
    {
        public override void Process(Page file)
        { 
            //not inversed for 'faster' check
            if (file.MdContent.Contains("</plate>"))
            {
                var tokenExtSettings = file.Book.Configuration.GetExtensionSettings<ImageTokenExtensionSettings>();

                foreach (var plate in file.HtmlDocument.DocumentNode.Descendants("plate").ToArray())
                {
                    var img = plate.GetAttributeValue("img","");
                    var name = plate.GetAttributeValue("name", "");
                    var subtype = plate.GetAttributeValue("subtype", "");
                    var html = plate.InnerHtml;

                    if(plate.Descendants("p").Count()>0)
                    {
                        html = plate.Descendants("p").FirstOrDefault().InnerHtml;
                    }

                    string stats = "";

                    plate.GetAttributeValue("stats", "")
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(statstr => {
                            var ststr = statstr.Trim();
                            if (ststr.StartsWith("-") || ststr.StartsWith("+"))
                            {
                                var num = Regex.Match(ststr, @"-?\d+").Value;

                                if (!num.Contains("-"))
                                    num = "+" + num;

                                return new
                                {
                                    stat = ststr.Replace(num, ""),
                                    mod = num
                                };
                            }
                            else return new
                            {
                                stat = "",
                                mod = ststr
                            };
                        })
                        .ForEach(statval =>
                        {
                            var img = statval.stat;

                            if(tokenExtSettings.TokenMappings.ContainsKey(img))
                                img = tokenExtSettings.TokenMappings[img];

                            if (stats.IsNotEmpty())
                                stats += " ";
                            stats += $"{statval.mod}{ImageTokenExtension.Token(24, file, "", img)}";
                        });

                    if (stats.IsNotEmpty())
                    {
                        stats = "<div class=\"subtype text\">" + stats + "</div";
                    }



                    var type = plate.GetAttributeValue("type", "");
                    if (type.IsNotEmpty())
                    {
                        if (type == "class")
                            stats = MakeClassStats(file,plate);
                    }
                    
                    var div = file.HtmlDocument.CreateElement("div");
                    div.AddClass("trait");
                    div.InnerHtml = PanelInnerHtml(img, subtype, name, html,file, stats);


                    plate.ParentNode.ReplaceChild(div, plate);

                    if (type.IsEmpty())
                    {
                        var h4 = file.HtmlDocument.CreateElement("h4");
                        h4.AddClass("wowplate-header");
                        h4.Id = name.Replace(" ", "-");

                        var a = file.HtmlDocument.CreateElement("a");
                        a.AddClass("header");
                        a.SetAttributeValue("href", "#" + h4.Id);
                        a.InnerHtml = name;

                        h4.AppendChild(a);
                        div.ParentNode.InsertBefore(h4, div);
                    }
                }

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;

                /*
                 * <div class="subtype text">+2 <img width="24" height="24" class="dice" src="./../../images/stats/end.png" alt="ВЫН"> -2 <img width="24" height="24" class="dice" src="./../../images/stats/cha.png" alt="ХАР"></div>
                 */
            }
        }

        private string PanelInnerHtml(string img, string subtype, string name, string html, Page page, string stats) => @$"
    <div class=""wowhead-tooltip"">
      <table class=""wrap-table"">
          <tbody>
            <tr>
                <td class=""wow-tp-td-content"">
                  <table class=""wow-tp-content-title-table"" style=""width: 100%;"">
                      <tbody>
                        <tr>
                            <td>
    <div class=""wow-icon"" style=""float:left"">
      <div class=""iconlarge"" data-env=""live"" data-tree=""live"" data-game=""wow"">    
        <ins style=""background-image: url('{page.PathToRoot}/images/icons/{img}');"" class="""">
        </ins>  
        <del>  
        </del>
      </div>
    </div>
                              <span class=""whtt-name"">
<h6 style=""display: inline-block;margin: 0;"">
<b class=""whtt-name"">{name}</b>
</h6>
                              <div class=""subtype"">{subtype}</div>
{stats}
                            </td>
                        </tr>
                      </tbody>
                  </table>
                  <table class=""wow-tp-content-description-table"">
                      <tbody>
                        <tr>
                            <td>
                              <div class=""text"">
                                {html} <br>
                              </div>
                            </td>
                        </tr>
                      </tbody>
                  </table>
                </td>
                <th class=""wow-tp-th-content""></th>
            </tr>
            <tr class=""wow-tp-bottom-tr"">
                <th class=""wow-tp-bottom"" style=""background-position: bottom left""></th>
                <th class=""wow-tp-bottom-corner"" style=""background-position: bottom right""></th>
            </tr>
          </tbody>
      </table>
    </div>";

        private string MakeClassStats(Page page, HtmlNode plate)
        {
            string html = "";

            var hitdice = plate.GetAttributeValue("hitdice", "");
            if(hitdice.IsNotEmpty())
            {
                html += "<div class=\"subtype text\">Кость здоровья: " + hitdice/*DiceExtension.Dice(page,Enum.Parse(typeof(Dice),hitdice).As<Dice>(),24) */+ "</div>";
            }

            var sp1 = plate.GetAttributeValue("sp1", "");
            if (sp1.IsNotEmpty())
            {
                html += "<div class=\"text\">Очки навыков: " + sp1 + "</div>";
            }

            var sp = plate.GetAttributeValue("sp", "");
            if (sp.IsNotEmpty())
            {
                html += "<div class=\"text\">Очки навыков каждый ур.: " + sp + "</div>";
            }

            var skills = plate.GetAttributeValue("skills", "");
            if (skills.IsNotEmpty())
            {
                html += "<div class=\"text\"> Навыки: </div>"+"<div class=\"text\">";
                skills.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .ForEach(skill =>
                    {
                        html += SkillIcon(page, skill);
                    });
                html += "</div>";
            }

            return html;
        }

        private string SkillIcon(Page page, string skill)
        {
            var img = Settings.SkillsIconMappings[skill.Trim()];
            return @$"<a class=""wow-icon wow-icon-small"" href=""{page.PathToRoot}/skills/{img}.html"" title=""{skill.Trim()}"">
            <div class=""iconlarge"" data-env=""live"" data-tree=""live"" data-game=""wow"">    
                <ins style=""background-image: url('{page.PathToRoot}/images/skills/{img}.png');"" class="""">
                </ins>  
                <del>  
                </del>
            </div>
            </a>";
        }

        protected override void SetDependencies()
        {
            this.DependsOn<ImageTokenExtension>();
        }
    }
}
