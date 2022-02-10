using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

internal class TimePicker
{
    public TimePicker(By by)
    {
        TimePickerParent = TestDirector.WebDriver.FindElement(by);
    }

    public int Hour
    {
        set
        {
            var hour = new WebElement(TimePickerParent.FindElement(By.CssSelector("div.time24hr input.numInput.flatpickr-hour"))).TextField("number");
            hour.SendKeys(value.ToString());
        }
    }

    public int Minute
    {
        set
        {
            var minute = new WebElement(TimePickerParent.FindElement(By.CssSelector("div.time24hr input.numInput.flatpickr-minute"))).TextField("number");
            minute.SendKeys(value.ToString());
        }
    }

    private OpenQA.Selenium.IWebElement TimePickerParent { get; }
}