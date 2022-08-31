using DynamicConverters.Attributes;
using System.Reflection;

namespace DynamicConverters.Converters
{
    internal class ConversionContext<T>
    {
        private T source;

        public static ConversionContext<V> From<V>(V source)
        {
            return new ConversionContext<V>(source);
        }

        public ConversionContext(T source)
        {
            this.source = source;
        }

        public Converter<V> To<V>()
        {
            var nSpace = "DynamicConverters.Converters";
            var assembly = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Namespace == nSpace 
            && x.GetCustomAttribute(typeof(ConversionSourceAttribute)) != null).ToList();

            foreach (Type type in assembly)
            {
                var attr = type.GetCustomAttribute(typeof(ConversionSourceAttribute));

                if(attr != null)
                {
                    ConversionSourceAttribute conversionSourceAttr = (ConversionSourceAttribute)attr;

                    if(typeof(T) == conversionSourceAttr.SourceType)
                    {
                        var constructedType = type.MakeGenericType(typeof(V));
                        object? instantiatedType = Activator.CreateInstance(constructedType);

                        if(instantiatedType != null)
                        {
                            var converter = (Converter<V>)instantiatedType;
                            converter.SetSource(source);
                            return converter;
                        }
                    }
                }
            }

            throw new NotImplementedException($"Converter for target {typeof(V)} with source {source} has not been implemented.");
        }


    }
}
