using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
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
        /// The string that the subject is expected to have similar content as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void HaveSimilarContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .BeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        /// <summary>
        /// Asserts that a string does not have similar content disregarding formatting and casing.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected not to have similar content as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void NotHaveSimilarContentAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
            => assertions.Subject.WithoutFormatting()
                .Should()
                .NotBeEquivalentTo(expected.WithoutFormatting(), because, becauseArgs);

        /// <summary>
        /// Asserts that a string has similar xml content disregarding formatting.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected to have similar xml content as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void HaveSimilarXmlAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
        {
            assertions.Subject.WithXmlFormatting()
                .Should()
                .Be(expected.WithXmlFormatting(), because, becauseArgs);
        }

        /// <summary>
        /// Asserts that a string does not have similar xml content disregarding formatting.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected not to have similar xml content as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void NotHaveSimilarXmlAs(
            this StringAssertions assertions,
            string expected,
            string because = "",
            params string[] becauseArgs)
        {
            assertions.Subject.WithXmlFormatting()
                .Should()
                .NotBe(expected.WithXmlFormatting(), because, becauseArgs);
        }

        /// <summary>
        /// Asserts that a string has similar json content disregarding formatting.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected to have similar json contnet as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void HaveSimilarJsonAs(
           this StringAssertions assertions,
           string expected,
           string because = "",
           params string[] becauseArgs)
        {
            assertions.Subject.WithJsonFormatting()
                .Should()
                .Be(expected.WithJsonFormatting(), because, becauseArgs);
        }

        /// <summary>
        /// Asserts that a string dos not have similar json content disregarding formatting.
        /// </summary>
        /// <param name="assertions">The StringAssertions.</param>
        /// <param name="expected">
        /// The string that the subject is expected to not have similar json contnet as.
        /// </param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string, object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <paramref name="because"/>.
        /// </param>
        public static void NotHaveSimilarJsonAs(
           this StringAssertions assertions,
           string expected,
           string because = "",
           params string[] becauseArgs)
        {
            assertions.Subject.WithJsonFormatting()
                .Should()
                .NotBe(expected.WithJsonFormatting(), because, becauseArgs);
        }

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

        /// <summary>
        /// Returns the string with xml formatting.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string with xml formatting.</returns>
        public static string WithXmlFormatting(
            this string str)
        {
            var doc = new XmlDocument();
            doc.LoadXml(str);

            using var writer = new StringWriter();
            using var textWriter = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented,
            };
            doc.WriteTo(textWriter);

            return writer.ToString();
        }

        /// <summary>
        /// Returns the string with json formatting.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string with json formatting.</returns>
        public static string WithJsonFormatting(
            this string str)
        {
            var doc = JsonDocument.Parse(str);
            var writeOptions = new JsonWriterOptions
            {
                Indented = true,
            };

            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer, writeOptions);
            doc.WriteTo(writer);
            writer.Flush();

            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }
    }
}