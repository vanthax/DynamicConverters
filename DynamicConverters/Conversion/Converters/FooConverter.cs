using DynamicConverters.Attributes;
using DynamicConverters.Model;

namespace DynamicConverters.Conversion.Converters
{
    [ConversionSource(typeof(Foo))]
    internal class FooConverter<T> : Converter<T>
    {
        private new Foo source;

        [ConversionTarget(typeof(Bar))]
        public Bar ToBar()
        {
            Bar bar = new Bar();
            bar.Value = source.Value.ToCharArray();
            return bar;
        }
    }
}
