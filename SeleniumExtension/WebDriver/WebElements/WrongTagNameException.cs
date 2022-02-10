using System;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class WrongTagNameException : Exception
{
    private readonly string _tagNameActual;
    private readonly string _tagNameExpected;

    public WrongTagNameException(string tagNameExpected, string tagNameActual)
    {
        _tagNameExpected = tagNameExpected;
        _tagNameActual = tagNameActual;
    }

    public override string Message => $"tag-name expected [{_tagNameExpected}] but was [{_tagNameActual}]";
}