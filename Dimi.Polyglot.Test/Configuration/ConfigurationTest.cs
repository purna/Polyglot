using System.Linq;
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
            Assert.True(nonStandardCultures.First().ISOCode == "xy");
            Assert.True(nonStandardCultures.First().Description == "Non-standard 1 (xy)");
            Assert.True(nonStandardCultures.First().CultureAlias == "xy-XY");
            Assert.True(nonStandardCultures.Last().ISOCode == "yz");
            Assert.True(nonStandardCultures.Last().Description == "Non-standard 2 (yz)");
            Assert.True(nonStandardCultures.Last().CultureAlias == "yz-YZ");
        }
    }
}
