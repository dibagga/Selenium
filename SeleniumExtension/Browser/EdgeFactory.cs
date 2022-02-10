using System;
using OpenQA.Selenium.Remote;
using Six.Test.Selenium.Extension.Environment;
using Six.Test.Selenium.Extension.WebDriver;

namespace Six.Test.Selenium.Extension.Browser;

public class EdgeFactory : BrowserFactory
{
    public EdgeFactory(TestEnvironment t) : base(t)
    {
    }

    /// <inheritdoc />
    protected override string BrowserName => "Edge";

    /// <inheritdoc />
    public override IWebDriver Local()
    {
        throw new NotImplementedException("Edge not supported yet");
    }

    /// <inheritdoc />
    public override RemoteWebDriver Remote()
    {
        throw new NotImplementedException("Edge not supported yet");
    }
}