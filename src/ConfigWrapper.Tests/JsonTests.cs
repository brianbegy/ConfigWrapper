using System.Linq;
using FluentAssertions;
using JSONConfigWrapper;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ConfigWrapper.Tests
{
    public class JsonTests
    {
        private JsonConfigWrapper sut = new  JsonConfigWrapper($"{TestContext.CurrentContext.TestDirectory}../../../data/test.json");

        [Test]
        [TestCase("simplekey", "value", "foo")]
        [TestCase("simpleKey", "value", "value")]
        [TestCase("missingKey", "value", "value")]
        [TestCase("category.key", "value", "foo")]
        [TestCase("topcategory.subcategory.key", "value", "foo")]
        public void StringTests(string key, string defaultValue, string expected)
        {
            sut.Get(key, defaultValue).Should().Be(expected);
        }

        [Test]
        [TestCase("invalid-key", new[] { "pork", "beans" }, new[] { "pork", "beans" })]
        [TestCase("string-array-found", new[] { "pork", "beans" }, new[] { "spam", "spam", "spam", "spam", "spam", "baked beans", "spam" })]
        [TestCase("string-array-as-string", new[] { "pork", "beans" }, new[] { "spam", "spam", "spam", "spam", "spam", "baked beans", "spam" })]
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
