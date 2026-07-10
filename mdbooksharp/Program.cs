using Geranium.Reflection;
using mdbooksharplib;

var command = args.ElementAtOrDefault(0);

if (command.IsEmpty())
{
    BookWizard.GenerateOrInit();
} 
else if (command == "init")
{
    BookWizard.Init(args.ElementAtOrDefault(1));
}
else
{
    BookWizard.Generate(command);
}