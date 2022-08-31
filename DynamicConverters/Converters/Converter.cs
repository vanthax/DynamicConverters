using DynamicConverters.Attributes;
using System.Reflection;

namespace DynamicConverters.Converters
{

    internal class Converter
    {
        public static ConversionContext<T> From<T>(T source)
        {
            return new ConversionContext<T>(source);
        }
    }

    internal abstract class Converter<T>
    {
        protected object? source;

        public T Convert()
        {
            List<MethodInfo> methods = this.GetType().GetMethods().Where(x => x.ReturnType == typeof(T)).ToList();

            foreach(MethodInfo method in methods)
            {
                var attr = method.GetCustomAttribute(typeof(ConversionTargetAttribute));

                if (attr != null)
                {
                    ConversionTargetAttribute conversionTargetAttr = (ConversionTargetAttribute)attr;

                    if (conversionTargetAttr.TargetType == typeof(T))
                    {
                        object? result = method.Invoke(this, null);

                        if(result != null)
                            return (T)result;
                    }
                }
            }
            throw new NotImplementedException($"No conversion method found for target {typeof(T)} with source.");
        }

        public abstract void SetSource<V>(V source);
    }
}
