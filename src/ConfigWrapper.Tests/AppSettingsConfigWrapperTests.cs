using NUnit.Framework;
using System.Linq;

namespace ConfigWrapper.Tests
{

    [TestFixture]
    public class AppSettingsConfigWrapperTests
    {
        AppSettingsConfigWrapper sut = new AppSettingsConfigWrapper();

        [Test]
        [TestCase("invalid-key", 999, 999)]
        [TestCase("integer-found", 100, 999)]
        public void IntegerTests(string key, int defaultValue, int expectedValue)
        {
            var result = sut.Get<int>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalid-key", "foo", "foo")]
        [TestCase("string-found", "foo", "bar")]
        public void StringTests(string key, string defaultValue, string expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalid-key", 1.99D, 1.99D)]
        [TestCase("double-found", 1.99D, 1.09D)]
        public void DoubleTests(string key, double defaultValue, double expectedValue)
        {
            var result = sut.Get<double>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase("invalid-key",   new []{ 1.99D, 1.89D }, new[] { 1.99D, 1.89D })]
        [TestCase("double-array-found", new[] { 1.99D, 1.09D }, new[] { 1.99D, 1.89D })]
        public void DoubleArrayTests(string key, double[] defaultValue, double[] expectedValue)
        {
            var result = sut.Get<double>(key, defaultValue, new[] { ','});
            Assert.That(result.SequenceEqual(expectedValue), $"Expected {string.Join(",", expectedValue)} got {string.Join(",", result)}.");
        }

        [Test]
        [TestCase("invalid-key",new[] { "pork", "beans" }, new[] { "pork", "beans" })]
        [TestCase("string-array-found", new[] { "pork", "beans" }, new[] { "spam", "spam", "spam", "spam", "spam", "baked beans", "spam" })]
        public void StringArrayTests(string key, string[] defaultValue, string[] expectedValue)
        {
            var result = sut.Get<string>(key, defaultValue, new[] { ',' });
            Assert.That(result.SequenceEqual(expectedValue), $"Expected {string.Join(",", expectedValue)} got {string.Join(",", result)}.");
        }
        [Test]
        public void DecimalTests()
        {
            //decimal isn't valid for attributes, so we have to call the tests like this.  
            DecimalTests("invalid-key", 2.50m, 2.50m);
            DecimalTests("decimal-found", 2.50m, 1.09m);
        }

        public void DecimalTests(string key, decimal defaultValue, decimal expectedValue)
        {
            var result = sut.Get<decimal>(key, defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

    }
}
