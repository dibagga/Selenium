using System;
using SIX.SCS.Tools.Environment;

namespace Six.Test.Selenium.Extension.Environment;

public class TestEnvironment : BaseEnvironment
{
    public IApplication Application;
    public IAuthentication Authentication;
    public string BaseToolPath;
    public Uri BaseUrl;
    public SeleniumConfig SeleniumConfig;

    public static bool ForceLocal
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}