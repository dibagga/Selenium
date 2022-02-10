using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SIX.SCS.Tools.Helper;

namespace SIX.SCS.Tools.Test.Helper;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class UrlHelperTests
{
    private static IEnumerable<TestCaseData> UpdateUrlParamsTestCases
    {
        get
        {
            yield return new TestCaseData("/foo/bar", null, "/foo/bar").SetName("{m}: No param in url");
            yield return new TestCaseData("/foo/{bar}/bar", null, "/foo/{bar}/bar").SetName("{m}: No param provided (null)");
            yield return new TestCaseData("/foo/{bar}/bar", new Dictionary<string, string>(), "/foo/{bar}/bar").SetName("{m}: No param provided (empty)");
            yield return new TestCaseData("/foo/{bar}/bar", new Dictionary<string, string> { ["bar"] = "123" }, "/foo/123/bar").SetName("{m}: Param within url");
            yield return new TestCaseData("/foo/bar", new Dictionary<string, string> { ["bar"] = "123" }, "/foo/bar?bar=123").SetName("{m}: Param in empty query string");
            yield return new TestCaseData("/foo/bar?v=2", new Dictionary<string, string> { ["bar"] = "123" }, "/foo/bar?v=2&bar=123").SetName("{m}: Param in existing query string");
            yield return new TestCaseData("/foo/{a}/bar", new Dictionary<string, string> { ["a"] = "1", ["b"] = "2", ["c"] = "3" }, "/foo/1/bar?b=2&c=3").SetName("{m}: Multiple params");
        }
    }

    [TestCaseSource(nameof(UpdateUrlParamsTestCases))]
    public void UpdateUrlParams(string url, Dictionary<string, string> parameters, string expected)
    {
        UrlHelper.UpdateUrlParams(url, parameters).Should().Be(expected);
    }
}