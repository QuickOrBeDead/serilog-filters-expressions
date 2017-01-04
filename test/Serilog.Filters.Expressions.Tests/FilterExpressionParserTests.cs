﻿using System;
using Xunit;

namespace Serilog.Filters.Expressions.Tests
{
    public class FilterExpressionParserTests
    {
        [Theory]
        [InlineData("contains(@Message,'some text')", "contains(@Message,@\"some text\")")]
        [InlineData("AProperty", null)]
        [InlineData("AProperty = 'some text'", "Equal(AProperty,@\"some text\")")]
        [InlineData("AProperty = 42", "Equal(AProperty,42)")]
        [InlineData("@Properties['0'] = 42", "Equal(@Properties[@\"0\"],42)")]
        [InlineData("AProperty = null", "Equal(AProperty,null)")]
        [InlineData("AProperty <> 42", "NotEqual(AProperty,42)")]
        [InlineData("has(AProperty)", null)]
        [InlineData("not A", "Not(A)")]
        [InlineData("Not(1 = 2)", "Not(Equal(1,2))")]
        [InlineData("AProperty = 3 and Another < 12", "And(Equal(AProperty,3),LessThan(Another,12))")]
        [InlineData("@Timestamp", null)]
        [InlineData("1 + (2 + 3) * 4", "Add(1,Multiply(Add(2,3),4))")]
        [InlineData("AProperty.Another = 3", "Equal(AProperty.Another,3)")]
        [InlineData("AProperty /2/3 = 7", "Equal(Divide(Divide(AProperty,2),3),7)")]
        [InlineData(@"A = /a.*\/b/", @"Equal(A,/a.*\/b/)")]
        [InlineData("contains(AProperty, /[^0-9]/)", "contains(AProperty,/[^0-9]/)")]
        [InlineData("AProperty[0] = 1", "Equal(AProperty[0],1)")]
        [InlineData("AProperty[0] = note", "Equal(AProperty[0],note)")] // Faking the 'not'
        [InlineData("equal(AProperty[0].Description,1)", null)]
        [InlineData("equal(AProperty[?].Description,1)", null)]
        [InlineData("equal(AProperty[*].Description,1)", null)]
        [InlineData("equal(AProperty[ * ].Description,1)", "equal(AProperty[*].Description,1)")]
        [InlineData("AProperty like '%foo'", "_Internal_Like(AProperty,@\"%foo\")")]
        [InlineData("A is null", "_Internal_Is(A,null)")]
        [InlineData("A is null", "_Internal_Is(A,null)")]
        [InlineData("A IS NOT NULL", "_Internal_Is(A,Not(null))")]
        [InlineData("A is not null or B", "Or(_Internal_Is(A,Not(null)),B)")]
        [InlineData("@EventType = 0xC0ffee", "Equal(@EventType,12648430)")]
        public void ValidSyntaxIsAccepted(string input, string expected = null)
        {
            var roundTrip = FilterExpressionParser.Parse(input).ToString();
            Assert.Equal(expected ?? input, roundTrip);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\"Hello!\"")]
        [InlineData("@\"Hello!\"")]
        [InlineData("$FE6789E6")]
        [InlineData("AProperty == 'some text'")]
        [InlineData("AProperty != 42")]
        [InlineData("!(1 == 2)")]
        [InlineData("AProperty = 3 && Another < 12")]
        [InlineData("A = 99999999999999999999999999999999999999999999")]
        [InlineData("A = 0x99999999999999999999999999999999999999999999")]
        public void InvalidSyntaxIsRejected(string input)
        {
            Assert.Throws<ArgumentException>(() => FilterExpressionParser.Parse(input));
        }
    }
}
