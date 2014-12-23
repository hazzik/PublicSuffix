using ClassLibrary3;
using NUnit.Framework;

namespace Brandy.PublicSuffix.Test
{
    [TestFixture]
    public class TestFixture
    {
        [TestCaseSource(typeof (TestCaseSource), "Source"), Test]
        public void Test(string host, string expected)
        {
            var parser = DomainParser.FromFile("effective_tld_names.dat");
            var domain = parser.Parse(host);
            if (domain == null)
            {
                Assert.AreEqual(null, expected);
            }
            else
            {
                Assert.AreEqual(expected, domain.RegisterableDomain);
            }
        }
    }
}