using System;
using System.IO;
using System.Net;
using log4net;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver;

public class ScreenshotHelper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ScreenshotHelper));
    private static string _fullPath;
    private readonly WebDriver _webDriver;

    public ScreenshotHelper(WebDriver webDriver)
    {
        _webDriver = webDriver;
        SetScreenshotPath();
    }

    public FileInfo TakeScreenShot(string filePrefix = "")
    {
        Screenshot screenShot;

        try
        {
            screenShot = _webDriver.WrappedWebDriver.GetScreenshot();
            Logger.Debug($"Screenshot taken [{screenShot.AsByteArray.Length}] bytes");
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to take screenshot [{e.Message}]");
            return null;
        }

        try
        {
            var fullPathFileName = GenerateFullPathFileName(filePrefix);

            screenShot.SaveAsFile(fullPathFileName, ScreenshotImageFormat.Png);
            Logger.Info($"Screenshot saved [{fullPathFileName}]");
            return new FileInfo(fullPathFileName);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Failed to save screen shot [{ex.Message}] -> dump to log");
            Logger.DebugFormat("data:image/png;base64,{0}", screenShot);
        }

        return null;
    }

    private static string GenerateFullPathFileName(string filePrefix)
    {
        var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        if (!string.IsNullOrEmpty(filePrefix))
        {
            fileName = Path.GetFileName(string.Concat(filePrefix, "_", fileName));
        }

        var fullPathFileName = Path.Combine(_fullPath, $"{fileName}.png");
        return fullPathFileName;
    }

    private static void SetScreenshotPath()
    {
        if (AppContext.BaseDirectory == null)
        {
            throw new FileNotFoundException("BaseDirectory is null");
        }

        _fullPath = Path.Combine($@"\\{Dns.GetHostName()}\", AppContext.BaseDirectory.Replace(Path.VolumeSeparatorChar, '$'), @"Test\Selenium\Screenshots");
        try
        {
            Logger.Info($"folder [{_fullPath}] does not exist and will be created");
            Directory.CreateDirectory(_fullPath);
        }
        catch
        {
            Logger.Warn($"folder could not be created [{_fullPath}]");
        }
    }
}