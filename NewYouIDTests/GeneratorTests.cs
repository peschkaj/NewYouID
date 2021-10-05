using System;
using NewYouID;
using NUnit.Framework;

namespace NewYouIDTests
{
    public class GeneratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(UuidGeneratorFactory.Precision.Millisecond)]
        [TestCase(UuidGeneratorFactory.Precision.Microsecond)]
        public void GeneratorCreatesSequentialIds(UuidGeneratorFactory.Precision precision)
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(precision, utcDateTimeProvider: FakeUtcDateTimeProvider.Instance);
            var first = g.NextId();
            var second = g.NextId();
            
            Assert.Greater(second, first);
        }
    }
}