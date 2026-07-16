using Geranium;
using Geranium.Reflection;
using Markdig;
using mdbooksharplib.Books;

namespace mdbooksharplib.Extensions
{
    public abstract class MdBookExtension<T> : MdBookExtension
        where T : class, new()
    {

        public T Settings { get; set; } = new T();

        public override Type GetSettingsType() => typeof(T);

        public override void BindSettings(object settings) => Settings = settings.As<T>();
    }

    public abstract class MdBookExtension : ToposortType
    {
        public virtual bool IsGlobal => false;

        public virtual bool IsStaticHtmlApplicable => false;

        public Book Book { get; set; }

        public abstract void Process(Page file);

        public virtual string ProcessMd(Page file, string md) => md;

        public virtual string ProcessStaticHtml(string content) => content;

        public virtual Type GetSettingsType() => default;

        public virtual void BindSettings(object settings) { }

        public virtual void Init(Book book, MarkdownPipeline pipeline) { }

        public virtual void BeforeRender(Book book) { }
    }
}
