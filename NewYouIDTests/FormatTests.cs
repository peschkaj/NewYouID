using System;
using System.Data.Common;
using System.Linq;
using NewYouID;
using NUnit.Framework;

namespace NewYouIDTests
{
    public class FormatTests
    {
        private const ulong Low62Mask = 0b_00111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111;
        private const ulong Low40Mask = 0b_00000000_00000000_00000000_11111111_11111111_11111111_11111111_11111111;
        
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(UuidGeneratorFactory.Precision.Millisecond)]
        [TestCase(UuidGeneratorFactory.Precision.Microsecond)]
        public void GeneratedIdIs128Bits(UuidGeneratorFactory.Precision precision)
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(precision,
                utcDateTimeProvider: FakeUtcDateTimeProvider.Instance);
            var id = g.NextId();
            var bytes = id.ToByteArray();

            Assert.AreEqual(16, bytes.Length);
        }

        [TestCase(UuidGeneratorFactory.Precision.Millisecond)]
        [TestCase(UuidGeneratorFactory.Precision.Microsecond)]
        public void VersionCanBeRetrieved(UuidGeneratorFactory.Precision precision)
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(precision,
                utcDateTimeProvider: FakeUtcDateTimeProvider.Instance);
            var id = g.NextId();
            var high = (ulong)(id >> 64);
            var versionMask = (ulong)0b1111_0000_0000_0000;
            var version = (high & versionMask) >> 12;

            Assert.AreEqual(0b0111, version);
        }

        [TestCase(UuidGeneratorFactory.Precision.Millisecond)]
        [TestCase(UuidGeneratorFactory.Precision.Microsecond)]
        public void UuidVariantCanBeRetrieved(UuidGeneratorFactory.Precision precision)
        {
            var g = UuidGeneratorFactory.CreateUuidGenerator(precision,
                utcDateTimeProvider: FakeUtcDateTimeProvider.Instance);
            var id = g.NextId();
            var low = BitConverter.ToUInt64( id.ToByteArray().Take(8).ToArray());
            var variant = low >> 62;

            Assert.AreEqual(0b10, variant);
        }

        [TestCase(UuidGeneratorFactory.Precision.Millisecond)]
        [TestCase(UuidGeneratorFactory.Precision.Microsecond)]
        public void RandCanBeRetrieved(UuidGeneratorFactory.Precision precision)
        {
            ulong identifier = 0xABAD1DEA_C0A1E5CE;
            var g = UuidGeneratorFactory.CreateUuidGenerator(precision, identifier,
                FakeUtcDateTimeProvider.Instance);
            var id = g.NextId();
            var low = BitConverter.ToUInt64( id.ToByteArray().Take(8).ToArray());

            ulong rand;

            switch (precision)
            {
                case UuidGeneratorFactory.Precision.Millisecond:
                    rand = low & Low62Mask;
                    identifier &= Low62Mask;
                    break;
                case UuidGeneratorFactory.Precision.Microsecond:
                    rand = low & Low40Mask;
                    identifier &= Low40Mask;
                    break;
                default:
                    throw new NotImplementedException("There are only two precisions, how did you get here?");
            }
            
            Assert.AreEqual(identifier, rand);
        }
    }
}