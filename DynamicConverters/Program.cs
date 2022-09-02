using DynamicConverters.Conversion;
using DynamicConverters.Model;

namespace DynamicConverters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Foo foo = new Foo();
            foo.Value = "Hello World!";
            Console.WriteLine("Foo: " + foo.Value);

            Bar bar = Converter.From(foo).To<Bar>().Convert();
            Console.WriteLine("Bar (as string): " + new string(bar.Value));

            Foo foo2 = Converter.From(bar).To<Foo>().Convert();
            Console.WriteLine("Foo2: " + foo2.Value);

            Console.ReadKey();
        }
    }
}