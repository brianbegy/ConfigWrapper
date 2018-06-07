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

    }
}
