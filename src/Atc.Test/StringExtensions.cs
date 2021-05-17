using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Atc.Test
{
    public static class StringExtensions
    {
        /// <summary>
        /// Asserts that a string has similar content disregarding formatting and casing.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected to be similar in content to.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void BeSimilarContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .BeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        /// <summary>
        /// Asserts that a string has similar content disregarding formatting and casing.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected to be similar in content to.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void NotBeSimilarContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .NotBeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        /// <summary>
        /// Returns the string with formatting removed.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string with formatting removed.</returns>
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