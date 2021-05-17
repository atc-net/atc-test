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
        public void BeSimilarContentAs_Should_Compare_String_WithoutFormatting(
            string input,
            string expectedResult)
            => input
                .Should()
                .BeSimilarContentAs(expectedResult);
    }
}
