using System;

namespace NewYouID
{
    public static class UuidGeneratorFactory
    {
        public enum Precision
        {
            Millisecond,
            Microsecond
        }

        public static IUuidGenerator CreateUuidGenerator(Precision precision, ulong? identifier = null, IUtcDateTimeProvider utcDateTimeProvider = null)
        {
            if (precision == Precision.Millisecond)
            {
                return new MillisecondPrecisionGenerator(identifier, utcDateTimeProvider);
            }
            else
            {
                return new MicrosecondPrecisionGenerator(identifier, utcDateTimeProvider);
            }
            
        }
    }
}