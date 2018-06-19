using FluentAssertions;
using NUnit.Framework;

namespace ConfigWrapper.Tests
{
    [TestFixture]
    public class InMemoryConfigWrapperTests
    {
        private IWritableConfigWrapper sut = new InMemoryConfigWrapper();

        [OneTimeSetUp]
        public void Setup()
        {
            sut.Set("foo", "bar");
            sut.Set("badVal", "chicken");
            sut.Set("goodInt", 89);
            sut.Set("biff.baz", "biff.baz.value");
            sut.Set("biff.foo", "biff.foo.value");
        }

        [TestCase("foo", "baz", "bar")]
        [TestCase("notfound", "default", "default")]
        public void StringTests(string key, string defaultValue, string expected)
        {
            sut.Get<string>(key, defaultValue).Should().Be(expected);
        }

        [TestCase("goodInt", 17, 89)]
        [TestCase("badVal", 17, 17)]
        [TestCase("notfound", 17, 17)]
        public void IntegerTests(string key, int defaultValue, int expected)
        {
            sut.Get<int>(key, defaultValue).Should().Be(expected);
        }


        [Test]
        public void BadValTest()
        {
            var ex = Assert.Throws<System.Exception>(() => sut.Get<double>("badVal", 78, true));
            Assert.That(ex.Message, Does.StartWith($"Cannot cast 'chicken'"));
        }

        [Test]
        public void AllKeysTests()
        {
            sut.AllKeys().Should().BeEquivalentTo(new[] { "foo", "badVal", "goodInt", "biff.baz", "biff.foo" });
        }

        [Test]
        public void AllKeysPrefixTests()
        {
            sut.AllKeys("biff.").Should().BeEquivalentTo(new[] { "biff.baz", "biff.foo" });
        }
    }
}
