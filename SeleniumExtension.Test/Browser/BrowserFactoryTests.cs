using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using Six.Test.Selenium.Extension.Browser;
using Six.Test.Selenium.Extension.Environment;
using Six.Test.Selenium.Extension.WebDriver;

namespace Six.Test.Selenium.Extension.Test.Browser;

[TestFixture]
public class BrowserFactoryTests
{
    private class TestBrowserFactory : BrowserFactory
    {
        /// <inheritdoc />
        public TestBrowserFactory(TestEnvironment testEnvironment, string partialPathName) : base(testEnvironment)
        {
            BrowserName = partialPathName;
        }

        /// <inheritdoc />
        protected override string BrowserName { get; }

        public string GetBrowserProfilePathAccessor(string name)
        {
            return GetBrowserProfilePath(name);
        }

        /// <inheritdoc />
        public override IWebDriver Local()
        {
            return null;
        }

        /// <inheritdoc />
        public override RemoteWebDriver Remote()
        {
            return null;
        }
    }

    [Test]
    [TestCase("CertificateAuthentication", "Cert")]
    [TestCase("BlaBla", "NoCert")]
    public void GetBrowserProfilePath_returns_expected_path(string name, string expectedSuffix)
    {
        const string partialPathName = "partialPath";

        var testEnvironment = new TestEnvironment { BaseToolPath = "basePath" };
        var factory = new TestBrowserFactory(testEnvironment, partialPathName);

        var path = factory.GetBrowserProfilePathAccessor(name);

        path.Should().EndWith($"\\Environments\\Profile\\partialPath\\{expectedSuffix}");
    }
}