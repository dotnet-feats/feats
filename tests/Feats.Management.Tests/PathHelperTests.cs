
using System;
using System.Collections.Generic;
using Feats.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Feats.Management.Tests
{
    public class PathHelperTests
    {
        [Test]
        public void GivenPathAndName_WhenCombiningNameAndPath_ThenWeGetDirectoryPath()
        {
            PathHelper.CombineNameAndPath("path/hello", "name")
            .Should().Be($"path/hello/name");
        }

        [Test]
        public void GivenPathAndNameWithTrailingDelimiter_WhenCombiningNameAndPath_ThenWeGetDirectoryPath()
        {
            PathHelper.CombineNameAndPath("path/hello", "name/")
            .Should().Be($"path/hello/name");
        }

        [Test]
        public void GivenPathWithTrailingDelimiterAndName_WhenCombiningNameAndPath_ThenWeGetDirectoryPath()
        {
            PathHelper.CombineNameAndPath("path/hello/", "name")
            .Should().Be($"path/hello/name");
        }

        [Test]
        public void GivenEmptyPathAndNonEmptyName_WhenCombiningNameAndPath_ThenWeGetDirectoryPath()
        {
            PathHelper.CombineNameAndPath(string.Empty, "name")
            .Should().Be("name");
        }
        
        [Test]
        public void GivenEmtpyEverything_WhenCombiningNameAndPath_ThenWeGetEmptyDirectoryPath()
        {
            PathHelper.CombineNameAndPath(null, string.Empty)
            .Should().Be(string.Empty);
        }

        [Test]
        public void GivenPathAndEmptyName_WhenCombiningNameAndPath_ThenWeGetDirectoryPath()
        {
            PathHelper.CombineNameAndPath("path/hello", string.Empty)
            .Should().Be("path/hello");
        }

        [Test]
        public void GivenPathWithSections_WhenTranformingToPathLevels_ThenWeGetAllPathSections()
        {
            var path = "one/level/is/never/enough";

            PathHelper.TranformToPathLevels(path)
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "one",
                    "one/level",
                    "one/level/is",
                    "one/level/is/never",
                    "one/level/is/never/enough",
                });
        }

        [Test]
        public void GivenPathWithSectionsAndTrailingDelimiter_WhenTranformingToPathLevels_ThenWeGetAllPathSections()
        {
            var path = "one/level/";

            PathHelper.TranformToPathLevels(path)
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "one",
                    "one/level",
                });
        }

        [Test]
        public void GivenRootPath_WhenTranformingToPathLevels_ThenWeGetRootPath()
        {
            var path = "one";

            PathHelper.TranformToPathLevels(path)
                .Should()
                .BeEquivalentTo(new List<string>
                {
                    "one",
                });
        }
    }
}