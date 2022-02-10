using System;
using System.IO;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Six.Test.Selenium.Extension.Environment;
using Six.Test.Selenium.Extension.WebDriver;

namespace Six.Test.Selenium.Extension.Browser;

public sealed class FirefoxFactory : BrowserFactory
{
    private const string SystemEnvironmentVariableName = "PATH";
    private readonly FirefoxOptions _firefoxOptions;
    private readonly FirefoxProfile _profile;

    public FirefoxFactory(TestEnvironment testEnvironment) : base(testEnvironment)
    {
        _profile = new FirefoxProfile(GetBrowserProfilePath(TestEnvironment.Authentication.GetType().Name));
        _firefoxOptions = FirefoxOptions();
    }

    protected override string BrowserName => "Firefox";

    private static string BrowserExecutable => "firefox.exe";

    public override IWebDriver Local()
    {
        EnsureGeckoDriverPathIsSet();

        return new WebDriver.WebDriver(new FirefoxDriver(DriverPath, _firefoxOptions));
    }

    public override RemoteWebDriver Remote()
    {
        return new RemoteWebDriver(new Uri(TestEnvironment.SeleniumConfig.GridAddress), _firefoxOptions.ToCapabilities());
    }

    private void EnsureGeckoDriverPathIsSet()
    {
        var currentPath = System.Environment.GetEnvironmentVariable(SystemEnvironmentVariableName);

        System.Environment.SetEnvironmentVariable(SystemEnvironmentVariableName, currentPath + ";" + DriverPath);
    }

    private FirefoxOptions FirefoxOptions()
    {
        return new FirefoxOptions { Profile = _profile, BrowserExecutableLocation = Path.Combine(BaseTestToolPath, BrowserName, BrowserExecutable) };
    }
}