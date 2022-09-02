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

            Foo[] foos = new Foo[] {
                new Foo(){ Value = "I" },
                new Foo(){ Value = "am" },
                new Foo(){ Value = "alive" }
            };

            Bar[] bars = Converter.From(foos).To<Bar[]>().Convert(); //explicit

            Assert.Multiple(() => {
                Assert.That(new string(bars[0].Value), Is.EqualTo(foos[0].Value));
                Assert.That(new string(bars[1].Value), Is.EqualTo(foos[1].Value));
                Assert.That(new string(bars[2].Value), Is.EqualTo(foos[2].Value));
            });

            Foo[] foos2 = Converter.From(bars).To<Foo[]>().Convert(); //implicit

            Assert.Multiple(() => {
                Assert.That(foos[0].Value, Is.EqualTo(foos2[0].Value));
                Assert.That(foos[1].Value, Is.EqualTo(foos2[1].Value));
                Assert.That(foos[2].Value, Is.EqualTo(foos2[2].Value));
            });

        }
    }
}