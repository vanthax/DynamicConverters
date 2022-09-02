using DynamicConverters.Attributes;
using DynamicConverters.Conversion.Converters;
using System.Reflection;

namespace DynamicConverters.Conversion
{

    public class Converter
    {
        public static ConversionContext<T> From<T>(T source)
        {
            return new ConversionContext<T>(source);
        }
    }

    public abstract class Converter<T>
    {
        protected object? source;

        public virtual T Convert()
        {
            if(source == null)
                throw new Exception("Source value has not ben initialised.");

            FieldInfo field = this.GetType().GetField("source", BindingFlags.NonPublic | BindingFlags.Instance);

            if(field == null)
                throw new Exception("Converter implementation is missing source property.");

            field.SetValue(this, source);

            List<MethodInfo> methods = this.GetType().GetMethods().Where(x => x.ReturnType == typeof(T)).ToList();

            foreach (MethodInfo method in methods)
            {
                var attr = method.GetCustomAttribute(typeof(ConversionTargetAttribute));

                if (attr != null)
                {
                    ConversionTargetAttribute conversionTargetAttr = (ConversionTargetAttribute)attr;

                    if (conversionTargetAttr.TargetType == typeof(T))
                    {
                        object? result = method.Invoke(this, null);

                        if (result != null)
                            return (T)result;
                    }
                }
            }
            throw new NotImplementedException($"No conversion method found for target {typeof(T)} with source.");
        }

        public void SetSource<V>(V source)
        {
            var attr = this.GetType().GetCustomAttribute(typeof(ConversionSourceAttribute));

            if (attr == null)
                throw new Exception("Configuration error: This converter is missing [ConversionSource] attribute.");

            ConversionSourceAttribute conversionSourceAttr = (ConversionSourceAttribute)attr;

            if (typeof(V) == conversionSourceAttr.SourceType)
            {
                this.source = (V)source;
            }
        }
    }
}
