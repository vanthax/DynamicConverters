using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverters.Conversion.Converters
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">source type</typeparam>
    /// <typeparam name="V">target type array</typeparam>
    /// <typeparam name="K">target type</typeparam>
    internal class ArrayConverter<T,V,K> : Converter<V>
    {
        private Converter<K>? converter;

        public override V Convert()
        {
            return (V)(object)ConvertAll();
        }

        public K[] ConvertAll()
        {
            if(converter is null)
                throw new ArgumentNullException("converter");

            if (source is not Array)
                throw new ArgumentNullException("source");

            T[] sourceArray = source as T[];
            K[] resultArray = new K[sourceArray.Length];

            Type targetBaseType = typeof(T).GetElementType();

            for (int i = 0; i < sourceArray.Length; i++)
            {
                converter.SetSource(sourceArray[i]);
                object? result = converter.Convert();
                resultArray[i] = (K)result;
            }
            return resultArray;
        }
    }
}
