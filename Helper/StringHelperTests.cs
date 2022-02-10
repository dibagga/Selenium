using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SIX.SCS.Tools.Helper;

namespace SIX.SCS.Tools.Test.Helper;

[TestFixture]
public class StringHelperTests
{
    [TestCase("", 0, "")]
    [TestCase("", 1, "")]
    [TestCase("", 3, "")]
    public void Crop_EmptyString(string givenString, int size, string expectedString)
    {
        givenString.Crop(size).Should().Be(expectedString);
    }

    [TestCase("", 0, "")]
    [TestCase("aa", 0, "")]
    [TestCase("aaa", 0, "")]
    public void Crop_ZeroSize(string givenString, int size, string croppedString)
    {
        givenString.Crop(size).Should().Be(croppedString, "Zero size must return empty string for zero-size");
    }

    [TestCase("", -3)]
    [TestCase("aa", -3)]
    public void Crop_NegativeSize(string givenString, int size)
    {
        givenString.Invoking(x => x.Crop(size)).Should().Throw<ArgumentOutOfRangeException>("exception should be thrown");
    }

    [TestCase("a", 1, "a")]
    [TestCase("aa", 1, "a")]
    [TestCase("aa", 2, "aa")]
    [TestCase("aaa", 0, "")]
    public void Crop_ValidStringsAndSizes(string givenString, int size, string croppedString)
    {
        givenString.Crop(size).Should().Be(croppedString);
    }

    [TestCase("a", 2, "a")]
    [TestCase("aa", 8, "aa")]
    [TestCase("aaa", int.MaxValue, "aaa")]
    [TestCase("", int.MaxValue, "")]
    public void Crop_ValidStringsAndOverSizes(string givenString, int size, string croppedString)
    {
        givenString.Crop(size).Should().Be(croppedString);
    }

    [TestCase("", new[] { "" })]
    [TestCase("\n\n", new[] { "", "", "" })]
    [TestCase("\r\r\n\n\r", new[] { "", "", "" })]
    [TestCase("\r\r\r", new[] { "" })]
    public void SplitByLineSeparator_EmptyStrings(string givenString, IList<string> expectedString)
    {
        givenString.SplitByLineSeparator().Should().Equal(expectedString);
    }

    [TestCase("a", new[] { "a" })]
    [TestCase("a a a", new[] { "a a a" })]
    [TestCase("\naa\na", new[] { "", "aa", "a" })]
    [TestCase("a\naa\n", new[] { "a", "aa", "" })]
    [TestCase("a\naa\n", new[] { "a", "aa", "" })]
    [TestCase("a\r\ra\n\na\r", new[] { "aa", "", "a" })]
    [TestCase("\r\r\na\n\ra", new[] { "", "a", "a" })]
    public void SplitByLineSeparator_ValidStrings(string givenString, IList<string> expectedString)
    {
        givenString.SplitByLineSeparator().Should().Equal(expectedString);
    }

    [Test]
    public void Crop_EmptyCall()
    {
        "01234".Crop(3).Should().Be("012");
    }
}