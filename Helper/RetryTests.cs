using System;
using FluentAssertions;
using NUnit.Framework;
using SIX.SCS.Tools.Helper;

namespace SIX.SCS.Tools.Test.Helper;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class RetryTests
{
    [TestCase(1)]
    [TestCase(3)]
    public void RetryForFailedCallThrowsException(int retry)
    {
        FluentActions.Invoking(
                () => Retry.Until(
                    () => false,
                    () =>
                    {
                    },
                    "",
                    retry))
            .Should()
            .Throw<Exception>();
    }

    [TestCase(0)]
    public void RetryFailedZeroTimesThrowsException(int retry)
    {
        FluentActions.Invoking(
                () => Retry.Until(
                    () => false,
                    () =>
                    {
                    },
                    "",
                    retry))
            .Should()
            .Throw<Exception>();
    }

    [TestCase(1)]
    [TestCase(3)]
    public void RetryForPassedCallThrowsNoException(int retry)
    {
        FluentActions.Invoking(
                () => Retry.Until(
                    () => true,
                    () =>
                    {
                    },
                    "",
                    retry))
            .Should()
            .NotThrow();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void RetryZeroTimesThrowsException(bool successfulCall)
    {
        FluentActions.Invoking(
                () => Retry.Until(
                    () => successfulCall,
                    () =>
                    {
                    },
                    "",
                    0))
            .Should()
            .Throw<Exception>();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void RetryNegativeRetriesTimesThrowsException(bool successfulCall)
    {
        FluentActions.Invoking(
                () => Retry.Until(
                    () => successfulCall,
                    () =>
                    {
                    },
                    "",
                    -1))
            .Should()
            .Throw<Exception>();
    }
}