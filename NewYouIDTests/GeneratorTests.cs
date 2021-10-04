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
        public void Test1()
        {
            var g = new Generator();
            var first = g.NextID();
            var second = g.NextID();
            
            Assert.Greater(second, first);
        }
    }
}