using System;
using System.IO;
using log4net;
using OpenQA.Selenium.Remote;
using Six.Test.Selenium.Extension.Environment;
using Six.Test.Selenium.Extension.WebDriver;

namespace Six.Test.Selenium.Extension.Browser;

public abstract class BrowserFactory
{
    private const string Certificate = "CertificateAuthentication";
    private const string CertProfileSuffix = "Cert";
    private const string DriverSubPathName = "Driver";
    private const string EnvironmentPath = "Environments";
    private const string NoCertProfileSuffix = "NoCert";
    private const string ProfilePath = "Profile";
    private static readonly ILog Logger = LogManager.GetLogger(typeof(BrowserFactory));
    protected readonly TestEnvironment TestEnvironment;

    protected BrowserFactory(TestEnvironment testEnvironment)
    {
        TestEnvironment = testEnvironment;
    }

    protected string BaseTestToolPath => Path.GetFullPath(TestEnvironment.BaseToolPath);

    protected abstract string BrowserName { get; }
    protected string DriverPath => Path.Combine(BaseTestToolPath, DriverSubPathName);

    public static BrowserFactory GetBrowserFactory(TestEnvironment t)
    {
        return t.Browser switch
        {
            SIX.SCS.Tools.Environment.Browser.Chrome => new ChromeFactory(t),
            SIX.SCS.Tools.Environment.Browser.Firefox => new FirefoxFactory(t),
            SIX.SCS.Tools.Environment.Browser.Edge => new EdgeFactory(t),
            _ => throw new ArgumentOutOfRangeException($"no browser found for:[{t.Browser}]")
        };
    }

    public abstract IWebDriver Local();
    public abstract RemoteWebDriver Remote();

    protected string GetBrowserProfilePath(string name)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, EnvironmentPath, ProfilePath, BrowserName);
        fullPath = name.Contains(Certificate) ? Path.Combine(fullPath, CertProfileSuffix) : Path.Combine(fullPath, NoCertProfileSuffix);

        Logger.Info($"{BrowserName}-ProfilePath: [{fullPath}]");
        return fullPath;
    }
}