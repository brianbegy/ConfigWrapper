using NUnit.Framework;
using System.Linq;

namespace ConfigWrapper.Tests
{

    [TestFixture]
    public class WindowsRegistryConfigWrapperTests
    {
        IWritableConfigWrapper sut = new WindowsRegistryConfigWrapper();

        [OneTimeSetUp]
        public void Setup() {
            sut.Set<int>("HKCU/ConfigWrapper/Test/integerfound", 1234, true);
            sut.Set<string>("HKCU/ConfigWrapper/Test/stringfound", "stringfound", true);
            sut.Set<double>("HKCU/ConfigWrapper/Test/doublefound", 12345.67D, true);
            sut.Set<string>("HKCU/ConfigWrapper/Test/stringArrayFound", "spam|spam|spam|spam|spam|baked beans|spam");
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
                DeleteKey("HKCU/ConfigWrapper/Test/integerfound");
                DeleteKey("HKCU/ConfigWrapper/Test/stringfound");
                DeleteKey("HKCU/ConfigWrapper/Test/doublefound");
                DeleteKey("HKCU/ConfigWrapper/Test");
                DeleteKey("HKCU/ConfigWrapper");
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        [Test]
        [TestCase("HKCU/ConfigWrapper/Test/invalidkey", 999, 999)]
        [TestCase("HKCU/ConfigWrapper/Test/integerfound", 100, 1234)]
        public void IntegerTests(string key, int defaultValue, int expectedValue)
        {
            var result = sut.Get<int>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("HKCU/ConfigWrapper/Test/invalidkey", "foo", "foo")]
        [TestCase("HKCU/ConfigWrapper/Test/stringFound", "foo", "stringfound")]
        public void StringTests(string key, string defaultValue, string expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("HKCU/ConfigWrapper/Test/invalidkey", 1.99D, 1.99D)]
        [TestCase("HKCU/ConfigWrapper/Test/doublefound", 1.99D, 12345.67D)]
        public void DoubleTests(string key, double defaultValue, double expectedValue)
        {
            var result = sut.Get<double>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("HKCU/ConfigWrapper/Test/invalid-key", new[] { "pork", "beans" }, new[] { "pork", "beans" })]
        [TestCase("HKCU/ConfigWrapper/Test/stringArrayFound", new[] { "pork", "beans" }, new[] { "spam", "spam", "spam", "spam", "spam", "baked beans", "spam" })]
        public void StringArrayTests(string key, string[] defaultValue, string[] expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue, new[] { ',','|' });
            Assert.That(result.SequenceEqual(expectedValue), $"Expected {string.Join(",", expectedValue)} got {string.Join(",", result)}.");
        }

        [Test]
        [TestCase("HKCU/ConfigWrapper/Test/stringfound", 11, 1.09D)]
        public void DoubleBadCastTest(string key, double defaultValue, double expectedValue)
        {
            var ex = Assert.Throws<System.Exception>(() => sut.Get<double>(key, defaultValue, true));
            Assert.That(ex.Message, Does.StartWith($"Cannot cast 'stringfound'"));
        }

    }
}
