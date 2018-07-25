using NUnit.Framework;
using System.Linq;
using FluentAssertions;

namespace ConfigWrapper.Tests
{

    [TestFixture]
    public class WindowsRegistryConfigWrapperTests
    {
        WindowsRegistryConfigWrapper sut = new WindowsRegistryConfigWrapper("HKCU/ConfigWrapper/Test/");

        [OneTimeSetUp]
        public void Setup() {
            sut.Set<int>("integerfound", 1234, true);
            sut.Set<string>("stringfound", "stringfound", true);
            sut.Set<double>("doublefound", 12345.67D, true);
            sut.Set<string>("stringArrayFound", "spam|spam|spam|spam|spam|baked beans|spam");
        }

        private void DeleteKey(string key) {
            try
            {
                sut.Delete(key);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"failed to delete key: {key}.  ex: {ex.ToString()}");
            }
        }

        [OneTimeTearDown]
        public void Teadown() {
            try
            {
                DeleteKey("integerfound");
                DeleteKey("stringfound");
                DeleteKey("doublefound");
                sut.Delete(sut.RootKey,true);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [Test]
        [TestCase("invalidkey", 999, 999)]
        [TestCase("integerfound", 100, 1234)]
        public void IntegerTests(string key, int defaultValue, int expectedValue)
        {
            var result = sut.Get<int>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalidkey", "foo", "foo")]
        [TestCase("stringFound", "foo", "stringfound")]
        public void StringTests(string key, string defaultValue, string expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalidkey", 1.99D, 1.99D)]
        [TestCase("doublefound", 1.99D, 12345.67D)]
        public void DoubleTests(string key, double defaultValue, double expectedValue)
        {
            var result = sut.Get<double>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalid-key", new[] { "pork", "beans" }, new[] { "pork", "beans" })]
        [TestCase("stringArrayFound", new[] { "pork", "beans" }, new[] { "spam", "spam", "spam", "spam", "spam", "baked beans", "spam" })]
        public void StringArrayTests(string key, string[] defaultValue, string[] expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue, new[] { ',','|' });
            Assert.That(result.SequenceEqual(expectedValue), $"Expected {string.Join(",", expectedValue)} got {string.Join(",", result)}.");
        }

        [Test]
        [TestCase("stringfound", 11, 1.09D)]
        public void DoubleBadCastTest(string key, double defaultValue, double expectedValue)
        {
            var ex = Assert.Throws<System.Exception>(() => sut.Get<double>(key, defaultValue, true));
            Assert.That(ex.Message, Does.StartWith($"Cannot cast 'stringfound'"));
        }

        [Test]
        public void GetKeys()
        {
            var result = sut.AllKeys();
            result.Count().Should().Be(4);
            result.Contains(@"HKEY_CURRENT_USER\ConfigWrapper\Test\integerfound");
        }

        [Test]
        public void MissingKeyTest()
        {
            var ex = Assert.Throws<System.Exception>(() => sut.Get<double>(@"nosuchKey"));
            Assert.That(ex.Message, Does.StartWith($"No config value found"));
        }
    }
}
