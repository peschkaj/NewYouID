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
        
        [Test]
        public void MillisecondGenerationTimeCanBeReconstructed()
        {
            IUtcDateTimeProvider utcDateTimeProvider = FakeUtcDateTimeProvider.Instance;;
            var g = UuidGeneratorFactory.CreateUuidGenerator(UuidGeneratorFactory.Precision.Millisecond, utcDateTimeProvider: utcDateTimeProvider);
            var id = g.NextId();

            var ms = (utcDateTimeProvider.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

            var high = (ulong)(id >> 64);
            var idMs = (high & 0xFF_FF_FF_FF_F0_00_00_00) >> 12;
            idMs += (high | 0b1111_11111111_00000000_00000000) >> 16;

            Assert.AreEqual(ms, idMs);
        }
    }
}