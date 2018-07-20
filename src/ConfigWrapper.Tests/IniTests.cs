using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace ConfigWrapper.Tests
{
    public class IniTests
    {
        [Test]
        [TestCase("foo", "biff", "bar")]
        [TestCase("baz", "biff", "biff")]
        public void ForValidEntryWeGetString(string key, string defaultValue, string expected)
        {
            Console.WriteLine(TestContext.CurrentContext.TestDirectory);
            var sut = new IniConfigWrapper($"{TestContext.CurrentContext.TestDirectory}../../../data/test.ini");
            sut.Get(key, defaultValue).Should().Be(expected);
        }

        [Test]
        public void GetAllKeys()
        {
            var sut = new IniConfigWrapper($"{TestContext.CurrentContext.TestDirectory}../../../data/test.ini");
             sut.AllKeys().Length.Should().Be(2);
            sut.AllKeys().First().Should().Be("foo");
            sut.AllKeys().Last().Should().Be("Section.value");
        }

        [Test]
        public void MissingKeyTest()
        {
            var sut = new IniConfigWrapper($"{TestContext.CurrentContext.TestDirectory}../../../data/test.ini");
            var ex = Assert.Throws<System.Exception>(() => sut.Get<double>("nosuchKey"));
            Assert.That(ex.Message, Does.StartWith($"No config value found"));
        }
    }
}
