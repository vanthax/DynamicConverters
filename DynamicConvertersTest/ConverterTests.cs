using DynamicConverters.Conversion;
using DynamicConverters.Model;

namespace DynamicConvertersTest
{
    public class ConverterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Foo foo = new Foo();
            foo.Value = "Hello World!";
            Console.WriteLine("Foo: " + foo.Value);

            Bar bar = Converter.From(foo).To<Bar>().Convert();
            Console.WriteLine("Bar (as string): " + new string(bar.Value));

            Foo foo2 = Converter.From(bar).To<Foo>().Convert();
            Console.WriteLine("Foo2: " + foo.Value);

            Assert.Multiple(() => {
                Assert.That(foo.Value, Is.EqualTo("Hello World!"));
                Assert.That(new string(bar.Value), Is.EqualTo("Hello World!"));
                Assert.That(foo2.Value, Is.EqualTo("Hello World!"));
            });
        }
    }
}