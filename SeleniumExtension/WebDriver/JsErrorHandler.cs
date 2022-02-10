using System.Collections.Generic;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver;

public class JsErrorHandler
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(JsErrorHandler));
    private readonly WebDriver _webDriver;

    public JsErrorHandler(WebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public void EnsureNoErrors()
    {
        var ignore = new (string url, string msg)[]
        {
            ("/help/", "favicon"),
            ("/help/", "jQuery"),
            ("/help/", "TableFilter"),
            ("/help/", "TableFilter"),
            ("https://wes-idp-dev.np.six-group.com", string.Empty), // ignore all messages issued by WES (not our business)
            ("https://www.googletagmanager.com", "ERR_TUNNEL_CONNECTION_FAILED") // firewall blocks our GoogleAnalytics tracking (not an issue)
        };

        IEnumerable<LogEntry> logEntries = GetAndClearConsoleLog(LogLevel.Warning)
            .Where(x => ignore.Where(i => _webDriver.Url.Contains(i.url)).All(i => !x.Message.Contains(i.msg)))
            .ToArray();

        if (logEntries.Any())
        {
            // note: the URL should be used with caution. We might have been redirected or have navigated in the meanwhile.
            var jsException = new JsException(TestDirector.WebDriver.Url, logEntries);
            Logger.Warn(jsException.Message);
            throw jsException;
        }
    }

    private void ClearConsoleLog()
    {
        _webDriver.ExecuteScript("console.clear();");
    }

    private IEnumerable<LogEntry> GetAndClearConsoleLog(LogLevel minimumLevel = LogLevel.All)
    {
        try
        {
            var logs = TestDirector.WebDriver.Manage().Logs.GetLog(LogType.Browser).Where(x => x.Level >= minimumLevel).ToList().AsReadOnly();
            return logs;
        }
        finally
        {
            ClearConsoleLog();
        }
    }
}