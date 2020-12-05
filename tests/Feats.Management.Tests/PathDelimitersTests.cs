
using Feats.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests
{
    public class PathDelimitersTests
    {
        [Test]
        public void GivenAPath_WhenFindingFirstDelimiter_ThenWeReturnFirstDelimiter()
        {
            foreach(var delimiter in PathDelimiters.Delimiters)
            {
               PathDelimiters.First($"aaa{delimiter}aa{delimiter}asdas").Should().Be(delimiter);
               PathDelimiters.First($"aaa{delimiter}aa.sds").Should().Be(delimiter);
            }
        }
        
        [Test]
        public void GivenAPathWithNoSections_WhenFindingFirstDelimiter_ThenWeReturnDot()
        {
            PathDelimiters.First("aaa").Should().Be(".");
        }
        
        [Test]
        public void GivenEmptyPath_WhenFindingFirstDelimiter_ThenWeReturnDot()
        {
            PathDelimiters.First(string.Empty).Should().Be(".");
        }
    }
}