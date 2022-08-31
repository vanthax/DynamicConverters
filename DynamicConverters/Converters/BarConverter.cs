using DynamicConverters.Attributes;
using DynamicConverters.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverters.Converters
{
    [ConversionSource(typeof(Bar))]
    internal class BarConverter<T> : Converter<T>
    {
        private Bar bar;

        public override void SetSource<V>(V source)
        {
            if(source is Bar)
            {
                //generics shenanigans
                bar = (Bar)(object)source;
            }
        }

        [ConversionTarget(typeof(Foo))]
        public Foo ToFoo()
        {
            Foo foo = new Foo();
            foo.Value = System.Convert.ToString(bar.Value);
            return foo;
        }
    }
}
