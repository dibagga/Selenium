using System;
using System.Threading;
using log4net;

namespace SIX.SCS.Tools.Helper;

public static class Retry
{
    private const int DefaultNumberOfRetries = 60;
    private static readonly TimeSpan DefaultSleepTime = TimeSpan.FromMilliseconds(500);
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Retry));

    public static void Until(Func<bool> condition, Action retryAction, string failMessage, int numberOfRetries = DefaultNumberOfRetries)
    {
        for (var remainingAttempts = numberOfRetries; remainingAttempts > 0; remainingAttempts--)
        {
            try
            {
                if (condition())
                {
                    Logger.Debug($"condition [{condition.Method.Name}] successfully checked");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.Debug($"retrying because of [{e.Message}] - remaining attempts [{remainingAttempts}]");
            }

            Thread.Sleep(DefaultSleepTime);

            if (retryAction == null)
            {
                continue;
            }

            Logger.Debug($"performing retry action [{retryAction.Method.Name}]");
            retryAction();
        }

        FinishedRetry(failMessage, numberOfRetries);
    }

    private static void FinishedRetry(string failMessage, int numberOfRetries)
    {
        throw new Exception($"Retry reached max attempts [{numberOfRetries}] with error: [{failMessage}]");
    }
}