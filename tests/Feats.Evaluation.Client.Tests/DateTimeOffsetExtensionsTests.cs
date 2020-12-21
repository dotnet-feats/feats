using System;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Evaluation.Client.Tests
{
    public class DateTimeOffsetExtensionsTests
    {
        [Test]
        public void GivenADate_WhenCalling_ThenIHaveAnIsoFormat()
        {
            var date = DateTimeOffset.Now;
            var expected = date.ToString("O");
                
            date
                .ToIsoStopBuggingMeFormat()
                .Should().Be(expected);
        }
    }
}