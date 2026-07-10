# Extensions

mdbooksharp support extensions via .NET code. 

> Feel free to fork and make your own book extensions!

## Create your own extension
First or all, you need to create extension class derived from `MdBookExtension`:
```C#
internal class LuaExtension : MdBookExtension
{
    
}
```
If you need some configuration for your extension, create configuration class and derive from generic extension `MdBookExtension<>`:
```C#
internal class LuaExtensionConfig
{
    public string ScriptsFolder { get; set; }
}

internal class LuaExtension : MdBookExtension<LuaExtensionConfig>
{
    
}
```

## Settings
When mdbooksharp parse `settings.json`, it search ext configurations inside `Extensions` section, and match the same name as extension class name. 

So, lets add some configuration for our extension to settings.json file:
```json
{
   ...
  "Extensions": {
    "LuaExtension":{
      "ScriptsFolder":"lua"
    }
  }
}
```

## Extension points
mdbooksharp extension have 4 extension points:

### Initialization
Extensions initialize before SUMMARY.md parse. Use this override when you need run this code once at a generation:
 
```C#
internal class LuaExtensionConfig
{
    public string ScriptsFolder { get; set; }
}

internal class LuaExtension : MdBookExtension<LuaExtensionConfig>
{
    Script script;

    public override void Init(Book book, MarkdownPipeline pipeline)
    {
        if (this.Settings.ScriptsFolder.IsEmpty()) //check settings is up
            return;

        script = new Script(); //create new lua host

        // detect path to lua files from book object
        var path = Path.Combine(book.ProjectPath, this.Settings.ScriptsFolder);

        //read and execute all lua scripts inside directory
        var files = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                script.DoFile(file);
            }
            catch (Exception ex)
            {
                ConsoleLog.Error(ex.ToString());
            }
        }
    }
}
```

### Markdown process
Use this override when you need change markdown markup before html generation:
 ```C#
internal class LuaExtension : MdBookExtension<LuaExtensionConfig>
{
    public override string ProcessMd(Page file, string md)
    {
        var regex = VariableRegexp();
        string result = regex.Replace(md, match => {
            string key = match.Value.Replace("//%(book.inj)", "");
            try
            {
                var val = script.DoString($"return {key}");
                var str = val.CastToString();
                return str;
            }
            catch (Exception ex)
            {
                ConsoleLog.Error(key);
                throw;
            }
        });

        return result;
    }
}
```

### Page process
Use this override when you want change output html markup:
 ```C#
internal class MDLinkToHtmlExtension : MdBookExtension<MDLinkToHtmlExtensionSettings>
{
    public override void Process(Page page)
    {
        if (page.Html.Contains(".md)"))
        {
            page.Html = page.Html.Replace(".md)", ".html)");
        }
    }
}
```

This override allow you to get all information about page:
* `page.MdContent` - contains original md markup
* `page.HtmlDocument` - contains `HtmlAgilityPack.HtmlDocument` object
* `page.Html` - contains plain html text
* `page.Book` - refrence to all book object

### Static html process
Use this override when you need to change content of static html files which will copy from src to bin directory:
 ```C#
internal class LuaExtension : MdBookExtension<LuaExtensionConfig>
{
    public override string ProcessStaticHtml(string content)
    {
        // same as ProcessMd
    }
}
```