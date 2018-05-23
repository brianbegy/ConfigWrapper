using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;

namespace ConfigWrapper.Tests
{


    [TestFixture]
    public class HelpersTests
    {

        [Test]
        [TestCase(null, 999, 999)]
        [TestCase(999, 100, 999)]

        public void IntegerTests(object input, int defaultValue, int expectedValue)
        {
            var result =
                input.CastAsT<int>(defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase(null, "foo", "foo")]
        [TestCase("bar", "foo", "bar")]
        public void StringTests(object input, string defaultValue, string expectedValue)
        {
            var result = input.CastAsT<string>(defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase(null, 1.99D, 1.99D)]
        [TestCase(1.09D, 1.99D, 1.09D)]
        public void DoubleTests(object input, double defaultValue, double expectedValue)
        {
            var result = input.CastAsT<double>(defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

        [Test]
        [TestCase(null, new [] {1.99D, 1.89D}, new[] { 1.99D, 1.89D })]
        [TestCase(new[] { 1.99D, 1.89D }, new[] { 1.99D, 1.09D }, new[] { 1.99D, 1.89D })]
        public void ArrayTests(object input, double[] defaultValue, double[] expectedValue)
        {
            var result = input.CastAsT<double[]>(defaultValue);
            Assert.That(result.SequenceEqual(expectedValue), $"Expected {string.Join(",", expectedValue)} got {string.Join(",", result)}.");
        }

        [Test]
        public void DecimalTests() {
            //decimal isn't valid for attributes, so we have to call the tests like this.  
            DecimalTests(null, 2.50m, 2.50m);
            DecimalTests(1.09m, 2.50m, 1.09m);
        }

        public void DecimalTests(object input, decimal defaultValue, decimal expectedValue)
        {
            var result = input.CastAsT<decimal>(defaultValue);
            Assert.That(result == expectedValue, $"Expected {expectedValue} got {result}.");
        }

    }
}
