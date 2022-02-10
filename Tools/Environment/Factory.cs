using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace SIX.SCS.Tools.Environment;

public class Factory<T> where T : BaseEnvironment
{
    private const string BrowserVariableName = "TestBrowser";
    private const string EnvironmentVariableName = "TestEnvironment";

    private static readonly ILog Logger = LogManager.GetLogger(typeof(Factory<T>));
    private readonly string _fileName;

    public Factory(string fileName)
    {
        _fileName = fileName;
        Data = EnvironmentData(GetEnvironmentToExecute());
    }

    public Factory(string fileName, Environment environment)
    {
        _fileName = fileName;
        Data = EnvironmentData(environment);
        Console.Out.WriteLine(Data);
    }

    public T Data { get; }

    private static Browser GetBrowserToExecute()
    {
        var browserVariableName = System.Environment.GetEnvironmentVariable(BrowserVariableName);
        var browser = (Browser)Enum.Parse(
            typeof(Browser),
            browserVariableName ?? throw new InvalidOperationException($"Browser environment [{BrowserVariableName}] variable not set"));
        Logger.Info($"browser set to [{browser}]");
        return browser;
    }

    private static Environment GetEnvironmentToExecute()
    {
        var environment = System.Environment.GetEnvironmentVariable(EnvironmentVariableName);
        Logger.Info($"environment determined [{environment}]");

        return environment switch
        {
            "DEV" => Environment.Dev,
            "ITU" => Environment.Itu,
            "ETU" => Environment.Etu,
            "PROD" => Environment.Prod,
            _ => Environment.Dev
        };
    }

    private T EnvironmentData(Environment environment)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, "Environments", _fileName);

        var environmentJson = JsonConvert.DeserializeObject<List<T>>(
            File.ReadAllText(fullPath),
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        Logger.Info($"Loaded JSon config-file [{fullPath}]");

        var environmentData =
            (environmentJson ?? throw new InvalidOperationException($"could not load environment [{_fileName}]")).First(e => e.Environment == environment);

        environmentData.Browser = GetBrowserToExecute();

        Logger.Info($"Test environment Data loaded [{environmentData}]");
        return environmentData;
    }
}