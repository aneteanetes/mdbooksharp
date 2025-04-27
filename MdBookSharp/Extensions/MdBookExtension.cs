using Geranium.Reflection;
using MdBookSharp.Books;

namespace MdBookSharp.Extensions
{
    internal abstract class MdBookExtension<T> : MdBookExtension
        where T : class, new()
    {
        public T Settings { get; set; } = new T();

        public override Type GetSettingsType() => typeof(T);

        public override void BindSettings(object settings) => Settings = settings.As<T>();
    }
    
    internal abstract class MdBookExtension
    {
        public abstract void Process(Page file);

        public abstract Type GetSettingsType();

        public abstract void BindSettings(object settings);
    }
}
