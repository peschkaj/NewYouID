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

        [Test]
        public void MillisecondGeneratorCreatesSequentialIds()
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(UuidGeneratorFactory.Precision.Millisecond);
            var first = g.NextId();
            var second = g.NextId();
            
            Assert.Greater(second, first);
        }

        [Test]
        public void MicrosecondGeneratorCreatesSequentialIds()
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(UuidGeneratorFactory.Precision.Microsecond);
            var first = g.NextId();
            var second = g.NextId();
            
            Assert.Greater(second, first);
        }
    }
}