using DynamicConverters.Attributes;
using System.Reflection;

namespace DynamicConverters.Conversion
{
    public class ConversionContext<T>
    {
        private T source;
        private List<Type> converterTypes;
        private Assembly[]? assemblies;
        private string? nSpace;

        public ConversionContext(T source)
        {
            this.source = source;
            converterTypes = new List<Type>();
        }

        public ConversionContext<T> Use(Assembly[] assemblies)
        {
            this.assemblies = assemblies;
            return this;
        }

        public ConversionContext<T> Use(string nSpace)
        {
            this.nSpace = nSpace;
            return this;
        }

        public Converter<V> To<V>()
        {
            //use current assembly if nothing else has been specified
            if (assemblies == null)
            {
                assemblies = new Assembly[1];
                assemblies[0] = Assembly.GetExecutingAssembly();
            }

            foreach (Assembly assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(x => x.IsClass && x.GetCustomAttribute(typeof(ConversionSourceAttribute)) != null);

                //restrict to specific namespaces
                if (nSpace != null)
                    types = types.Where(x => x.Namespace == nSpace);

                types = types.ToList();

                foreach (Type type in types)
                {
                    var attr = type.GetCustomAttribute(typeof(ConversionSourceAttribute));

                    if (attr != null)
                    {
                        ConversionSourceAttribute conversionSourceAttr = (ConversionSourceAttribute)attr;

                        if (typeof(T) == conversionSourceAttr.SourceType)
                        {
                            converterTypes.Add(type);
                        }
                    }
                }
            }

            if (converterTypes == null)
                throw new NotImplementedException($"No converter found for {source}.");

            if(converterTypes.Count == 1)
                return InstantiateConverter<V>(converterTypes[0]);

            //Multiple converters for a single source may exist, but targets must be declared only once for each source.
            Converter<V>? actualConverter = null;
            foreach (Type converterType in converterTypes)
            {
                List<MethodInfo> methods = converterType.GetType().GetMethods().Where(x => x.ReturnType == typeof(T)).ToList();

                foreach (MethodInfo method in methods)
                {
                    var attr = method.GetCustomAttribute(typeof(ConversionTargetAttribute));

                    if (attr != null)
                    {
                        ConversionTargetAttribute conversionTargetAttr = (ConversionTargetAttribute)attr;

                        if (conversionTargetAttr.TargetType == typeof(T))
                        {
                            if (actualConverter != null)
                                throw new Exception($"Ambigous configuration! Multiple target methods for target {typeof(V)} with source {source} have been identified.");

                            actualConverter = InstantiateConverter<V>(converterType);
                        }
                    }
                }
            }

            if(actualConverter != null)
                return actualConverter;

            throw new NotImplementedException($"no converter method for target {typeof(V)} with source {source} has not been implemented.");
        }

        private Converter<X> InstantiateConverter<X>(Type converterType)
        {
            var constructedType = converterType.MakeGenericType(typeof(X));
            object? instantiatedType = Activator.CreateInstance(constructedType);

            if (instantiatedType != null)
            {
                var converter = (Converter<X>)instantiatedType;
                converter.SetSource(source);
                return converter;
            }
            throw new Exception($"Error creating new instance of converter for source {source} and target {typeof(X)}");
        }
    }
}
