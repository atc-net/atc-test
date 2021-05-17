using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Atc.Test
{
    public static class StringExtensions
    {
        public static void BeSameContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .BeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        public static void NotBeSameContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .NotBeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        public static string WithoutFormatting(
            this string str)
            => Regex.Replace(
                str.Trim(),
                "[\\s\\n]+",
                " ",
                RegexOptions.None,
                TimeSpan.FromSeconds(1));
    }
}