using DynamicConverters.Attributes;
using DynamicConverters.Model;

namespace DynamicConverters.Conversion.Converters
{
    [ConversionSource(typeof(Bar))]
    internal class BarConverter<T> : Converter<T>
    {
        private new Bar source;

        [ConversionTarget(typeof(Foo))]
        public Foo ToFoo()
        {
            Foo foo = new Foo();
            foo.Value = new string(source.Value);
            return foo;
        }
    }
}
