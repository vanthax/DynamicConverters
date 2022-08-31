using DynamicConverters.Attributes;
using DynamicConverters.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverters.Converters
{
    [ConversionSource(typeof(Foo))]
    internal class FooConverter<T> : Converter<T>
    {
        private Foo foo;

        public override void SetSource<V>(V source)
        {
            if(source is Foo)
            {
                //generics shenanigans
                foo = (Foo)(object)source; 
            }
        }

        [ConversionTarget(typeof(Bar))]
        public Bar ToBar()
        {
            Bar bar = new Bar();
            bar.Value = foo.Value.ToCharArray();
            return bar;
        }
    }
}
