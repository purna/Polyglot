using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Dimi.Polyglot.Test.Configuration
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfiguration()
        {
            var nonStandardCultures = Dimi.Polyglot.Configuration.Configuration.NonStandardCultures;
            Assert.True(nonStandardCultures.Count == 2);
            Assert.True(nonStandardCultures.First().ISOCode == "n1");
            Assert.True(nonStandardCultures.First().Description == "Non-standard 1");
            Assert.True(nonStandardCultures.First().CultureAlias == "Non-standard alias 1");
            Assert.True(nonStandardCultures.Last().ISOCode == "n2");
            Assert.True(nonStandardCultures.Last().Description == "Non-standard 2");
            Assert.True(nonStandardCultures.Last().CultureAlias == "Non-standard alias 2");
        }
    }
}
