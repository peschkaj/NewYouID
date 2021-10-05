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

        public static IUuidGenerator CreateUuidGenerator(Precision precision, ulong? identifier = null)
        {
            if (precision == Precision.Millisecond)
            {
                if (identifier.HasValue)
                {
                    return new MillisecondPrecisionGenerator(identifier.Value);
                }

                return new MillisecondPrecisionGenerator();
            }
            else
            {
                if (identifier.HasValue)
                {
                    return new MicrosecondPrecisionGenerator(identifier.Value);
                }

                return new MicrosecondPrecisionGenerator();
            }
            
        }
    }
}