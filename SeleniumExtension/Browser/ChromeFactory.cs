using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Six.Test.Selenium.Extension.Environment;
using IWebDriver = Six.Test.Selenium.Extension.WebDriver.IWebDriver;

namespace Six.Test.Selenium.Extension.Browser;

internal sealed class ChromeFactory : BrowserFactory
{
    private readonly string chromeDriverPath;
    private readonly ChromeOptions options;

    public ChromeFactory(TestEnvironment t) : base(t)
    {
        options = ChromeOptions();
        chromeDriverPath = Path.Combine(BaseTestToolPath, DriverPath);
        options.Proxy = new Proxy { Kind = ProxyKind.System };
        options.AddArgument("--window-size=1300,900");
    }

    protected override string BrowserName => "Chrome";

    public override IWebDriver Local()
    {
        return new WebDriver.WebDriver(new ChromeDriver(chromeDriverPath, options));
    }

    public override RemoteWebDriver Remote()
    {
        return new RemoteWebDriver(new Uri(TestEnvironment.SeleniumConfig.GridAddress), options.ToCapabilities());
    }

    private static ChromeOptions ChromeOptions()
    {
        return new ChromeOptions { Proxy = new Proxy { IsAutoDetect = true } };
    }
}