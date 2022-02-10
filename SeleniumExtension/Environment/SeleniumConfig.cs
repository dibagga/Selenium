using System.Diagnostics.CodeAnalysis;

namespace Six.Test.Selenium.Extension.Environment;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Needed by deserializer")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Needed by deserializer")]
public class SeleniumConfig
{
    public string GridAddress { get; set; }
}