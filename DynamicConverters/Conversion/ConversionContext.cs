using DynamicConverters.Attributes;
using DynamicConverters.Conversion.Converters;
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
            FindConverters(typeof(T));

            //no converters found and type is not an array
            if (converterTypes == null && source is not Array)
                throw new NotImplementedException($"No converter found for {source}.");

            //we found a single explicitly implemented converter for this type
            if(converterTypes.Count == 1)
                return InstantiateConverter<V>(converterTypes[0]);

            //we found a multiple explicitly implemented converter for this type
            if (converterTypes.Count > 1)
                return HandleMultipleResults<V>();

            //we found no explicitly implemented converter for the array type, but what can we maybe
            //wrap the base type in a generic converter for arrays?
            if (source is Array)
                return HandleImplicitArrays<V>();

            throw new NotImplementedException($"no converter method for target {typeof(V)} with source {source} has not been implemented.");
        }

        public object To(Type targetType)
        {
            var sourceType = this.source.GetType();

            FindConverters(sourceType);

            if (converterTypes.Count == 1)
            {
                return InstantiateConverter(converterTypes[0], targetType);
            }

            throw new NotImplementedException($"no converter method for target {targetType} with source {source} has not been implemented.");
        }

        private void FindConverters(Type sourceType)
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

                        if (sourceType == conversionSourceAttr.SourceType)
                        {
                            converterTypes.Add(type);
                        }
                    }
                }
            }
        }

        //won't set source!
        private object InstantiateConverter(Type converterType, Type targetType)
        {
            var constructedType = converterType.MakeGenericType(targetType);
            object? instantiatedType = Activator.CreateInstance(constructedType);

            if (instantiatedType != null)
            {
                FieldInfo field = instantiatedType.GetType().GetField("source", BindingFlags.NonPublic | BindingFlags.Instance);
                return instantiatedType;
            }
            throw new Exception($"Error creating new instance of converter for source {source} and target {targetType}");
        }

        private Converter<V> InstantiateConverter<V>(Type converterType)
        {
            var constructedType = converterType.MakeGenericType(typeof(V));
            object? instantiatedType = Activator.CreateInstance(constructedType);

            if (instantiatedType != null)
            {
                var converter = (Converter<V>)instantiatedType;
                converter.SetSource(source);
                return converter;
            }
            throw new Exception($"Error creating new instance of converter for source {source} and target {typeof(V)}");
        }

        private Converter<V> HandleImplicitArrays<V>()
        {
            if(typeof(T) == typeof(Array) && typeof(V) != typeof(Array))
            {
                throw new Exception($"Implicit array to element conversion is not supported.");
            }
            Type sourceBaseType = this.source.GetType().GetElementType();
            Type targetBaseType = typeof(V).GetElementType();

            if(sourceBaseType == null || targetBaseType == null)
                throw new Exception($"Could not obtain base types.");

            var constructedType = typeof(ArrayConverter<,,>)
                .MakeGenericType(sourceBaseType, typeof(V), targetBaseType);

            object? instantiatedType = Activator.CreateInstance(constructedType);

            if (instantiatedType != null)
            {
                var baseConverter = Converter.From(Activator.CreateInstance(sourceBaseType)).To(targetBaseType);
                MethodInfo baseMethod = null;
                List<MethodInfo> methods = baseConverter.GetType().GetMethods().Where(x => x.ReturnType == targetBaseType).ToList();

                foreach (MethodInfo method in methods)
                {
                    var attr = method.GetCustomAttribute(typeof(ConversionTargetAttribute));

                    if (attr != null)
                    {
                        ConversionTargetAttribute conversionTargetAttr = (ConversionTargetAttribute)attr;

                        if (conversionTargetAttr.TargetType == targetBaseType)
                        {
                            baseMethod = method;
                        }
                    }
                }

                //inject converter, method and source in our array converter
                FieldInfo field = instantiatedType.GetType().GetField("converter", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(instantiatedType, baseConverter);

                field = instantiatedType.GetType().GetField("source", BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(instantiatedType, source);

                //return result
                return instantiatedType as Converter<V>;
            }
            throw new Exception($"Error creating new instance of generic array converter for source {source} and target {typeof(V)}");
        }

        private Converter<V> HandleMultipleResults<V>()
        {
            Converter<V>? actualConverter = null;

            //Multiple converters for a single source may exist, but targets must be declared only once for each source.
            foreach (Type converterType in converterTypes)
            {
                List<MethodInfo> methods = converterType.GetType().GetMethods().Where(x => x.ReturnType == typeof(T)).ToList();

                foreach (MethodInfo method in methods)
                {
                    var attr = method.GetCustomAttribute(typeof(ConversionTargetAttribute));

                    if (attr != null)
                    {
                        ConversionTargetAttribute conversionTargetAttr = (ConversionTargetAttribute)attr;

                        if (conversionTargetAttr.TargetType == typeof(V))
                        {
                            if (actualConverter != null)
                                throw new Exception($"Ambigous configuration! Multiple target methods for target {typeof(V)} with source {source} have been identified.");

                            actualConverter = InstantiateConverter<V>(converterType);
                        }
                    }
                }
            }
            throw new NotImplementedException($"no converter method for target {typeof(V)} with source {source} has not been implemented.");
        }
    }
}
