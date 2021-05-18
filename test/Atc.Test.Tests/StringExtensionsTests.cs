using System;
using FluentAssertions;
using Xunit;

namespace Atc.Test.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("Double  Space", "Double Space")]
        [InlineData("Triple   Space", "Triple Space")]
        [InlineData("Line \n Break", "Line Break")]
        [InlineData(" \n  Leading spaces", "Leading spaces")]
        [InlineData("Trailing spaces  \n ", "Trailing spaces")]
        public void WithoutFormatting_Should_Remove_Duplicate_Whitespaces_And_Trim(
            string input,
            string expectedResult)
            => input.WithoutFormatting()
                .Should()
                .Be(expectedResult.WithoutFormatting());

        [Theory]
        [InlineData("Double  Space", "Double Space")]
        [InlineData("Triple   Space", "Triple Space")]
        [InlineData("Line \n Break", "Line Break")]
        [InlineData(" \n  Leading spaces", "Leading spaces")]
        [InlineData("Trailing spaces  \n ", "Trailing spaces")]
        public void HaveSimilarContentAs_Should_Compare_String_WithoutFormatting(
            string input,
            string expectedResult)
            => input
                .Should()
                .HaveSimilarContentAs(expectedResult);

        [Theory]
        [InlineData("<xml><a>test</a></xml>", "<xml>\r\t<a>test</a>\n</xml>")]
        [InlineData("<xml    test=\"test\"></xml>", "<xml test=\"test\"></xml>")]
        public void WithXmlFormatting_Should_Align_Xml_Formatting(
            string input,
            string expectedResult)
            => input.WithXmlFormatting()
                .Should()
                .Be(expectedResult.WithXmlFormatting());

        [Theory]
        [InlineData("<xml><a>test</a></xml>", "<xml>\r\t<a>test</a>\n</xml>")]
        [InlineData("<xml    test=\"test\"></xml>", "<xml test=\"test\"></xml>")]
        public void HaveSimilarXmlAs_Should_Compare_String_WithoutFormatting(
            string input,
            string expectedResult)
            => input
                .Should()
                .HaveSimilarXmlAs(expectedResult);

        [Theory]
        [InlineData("{}", "{  \n\t}")]
        [InlineData("[]", "[  \n\t]")]
        [InlineData("{\"test\":\"value\"}", "{\n\t\"test\": \"value\"\n}")]
        public void WithJsonFormatting_Should_Align_Xml_Formatting(
            string input,
            string expectedResult)
            => input.WithJsonFormatting()
                .Should()
                .Be(expectedResult.WithJsonFormatting());

        [Theory]
        [InlineData("{}", "{  \n\t}")]
        [InlineData("[]", "[  \n\t]")]
        [InlineData("{\"test\":\"value\"}", "{\n\t\"test\": \"value\"\n}")]
        public void HaveSimilarJsonAs_Should_Compare_String_WithoutFormatting(
            string input,
            string expectedResult)
            => input
                .Should()
                .HaveSimilarJsonAs(expectedResult);
    }
}
