using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver;

internal class JsException : Exception
{
    private readonly IEnumerable<LogEntry> _logEntries;
    private readonly string _url;

    public JsException(string url, IEnumerable<LogEntry> logEntries)
    {
        _url = url;
        _logEntries = logEntries;
    }

    public override string Message => $"did not expect JS issues (warns/ errors) in console on url '{_url}', but found:\n{string.Join("\n", _logEntries)}";
}