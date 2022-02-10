using System;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using SIX.SCS.Tools.Helper;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class DateTimePickerElement : TextFieldElement
{
    private static readonly By AnyDatePickerDiv = By.CssSelector("div.flatpickr-calendar");
    private static readonly By CurrentlyActiveDatePickerDiv = By.CssSelector("div.flatpickr-calendar.open");
    private static readonly ILog Logger = LogManager.GetLogger(typeof(DateTimePickerElement));
    private readonly bool _hasTimePicker;
    private readonly WebDriverWait _webDriverWait;
    private readonly TimeSpan _withinToleratedTimeSpan;

    public DateTimePickerElement(OpenQA.Selenium.IWebElement webElement) : this(webElement, true)
    {
    }

    protected DateTimePickerElement(OpenQA.Selenium.IWebElement webElement, bool hasTimePicker) : base(webElement, type: "hidden")
    {
        _hasTimePicker = hasTimePicker;
        _webDriverWait = TestDirector.WebDriver.Wait();
        var id = webElement.GetAttribute("id");
        CalendarToggleButton = TestDirector.WebDriver.FindAdaptedElement(By.CssSelector($"button[id^='{id}_open']")).Button();
        ClearInputButton = TestDirector.WebDriver.FindAdaptedElement(By.CssSelector($"#{id}_clearInput")).Button();
        PickerTextField = TestDirector.WebDriver.FindAdaptedElement(By.CssSelector($"input[id='{id}']+input")).TextField();
        _withinToleratedTimeSpan = hasTimePicker ? TimeSpan.FromMinutes(1) : TimeSpan.FromDays(1);
    }

    public override bool Displayed
    {
        get
        {
            try
            {
                return PickerTextField.Displayed;
            }
            catch (WebDriverException e)
            {
                Logger.Warn(e.Message);
                TestDirector.WebDriver.TakeScreenShot();
                throw;
            }
        }
    }

    private ButtonElement CalendarToggleButton { get; }
    private ButtonElement ClearInputButton { get; }
    private bool IsCalendarOpen => TestDirector.WebDriver.FindAdaptedElements(AnyDatePickerDiv).Any(p => p.Displayed);
    private TextFieldElement PickerTextField { get; }

    public void Pick(string dateTimeStr)
    {
        if (DateTime.TryParse(dateTimeStr, out var dateTime))
        {
            throw new ArgumentException($"Cannot parse dateTime string: {dateTimeStr}");
        }

        Pick(dateTime);
    }

    public void Pick(DateTime dateTime)
    {
        var dateToPick = _hasTimePicker ? dateTime : new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

        OpenCalendar();

        var calendarPicker = new CalendarPicker(CurrentlyActiveDatePickerDiv) { Year = dateToPick.Year, Month = dateToPick.Month, Day = dateToPick.Day };
        Logger.Debug($"calendar picker set [{calendarPicker}]");

        if (_hasTimePicker)
        {
            var timePicker = new TimePicker(CurrentlyActiveDatePickerDiv) { Hour = dateToPick.Hour, Minute = dateToPick.Minute };
            Logger.Debug($"time picker set [{timePicker}]");
        }

        CloseCalendar();

        EnsureTextFieldSetCorrectly(dateToPick);
    }

    public void Reset()
    {
        Logger.Info("Reset Calendar");

        ClearInputButton.Click(); // deletes any input even if read-only
    }

    private void CloseCalendar()
    {
        Logger.Debug("Close Calendar");

        if (IsCalendarOpen)
        {
            CalendarToggleButton.Click();
        }

        Retry.Until(() => !IsCalendarOpen, null, "Date picker calendar is still open", 10);
    }

    private void EnsureTextFieldSetCorrectly(DateTime dateTime)
    {
        var dateTimeFromTextField = DateTime.Parse(PickerTextField.Text);

        Logger.Info($"DateTime set on text-field[{dateTimeFromTextField}] - date-time[{dateTime}]");
        if (dateTime - dateTimeFromTextField > _withinToleratedTimeSpan)
        {
            throw new ArgumentOutOfRangeException($"DateTime set on text-field[{dateTimeFromTextField}] differs too much from given date-time[{dateTime}]");
        }
    }

    private void OpenCalendar()
    {
        Logger.Debug("Open Calendar");

        PickerTextField.Click();

        _webDriverWait.Message = "wait until DateTimePicker is shown";
        _webDriverWait.Until(ExpectedConditions.ElementIsVisible(CurrentlyActiveDatePickerDiv));
    }
}