using System;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class WrongInputTypeException : Exception
{
    private readonly string _typeActual;
    private readonly string _typeExpected;

    public WrongInputTypeException(string typeExpected, string typeActual)
    {
        _typeExpected = typeExpected;
        _typeActual = typeActual;
    }

    public override string Message => $"type expected [{_typeExpected}] but was [{_typeActual}]";
}