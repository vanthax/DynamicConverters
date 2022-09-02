using DynamicConverters.Attributes;
using DynamicConverters.Model;

namespace DynamicConverters.Conversion.Converters
{
    [ConversionSource(typeof(Foo[]))]
    internal class FooArrayConverter<T> : Converter<T>
    {
        private new Foo[] source;

        [ConversionTarget(typeof(Bar[]))]
        public Bar[] ToBar()
        {
            var converter = new FooConverter<Bar>();
            var result = new Bar[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                converter.SetSource(source[i]);
                result[i] = converter.Convert();
            }
            return result;
        }
    }
}
