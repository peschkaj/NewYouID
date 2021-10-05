using System.Numerics;

namespace NewYouID
{
    public abstract class AbstractGenerator : IUuidGenerator
    {
        protected const ulong Low62Mask = 0b_00111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111;
        protected const ulong Low40Mask = 0b_00000000_00000000_00000000_11111111_11111111_11111111_11111111_11111111;

        protected const ulong ThirtySixBitMask =
            0b_00000000_00000000_00000000_00001111_11111111_11111111_11111111_11111111;

        protected const ulong MicrosecondUnixTsMask =
            0b_00000000_00000000_00000000_00000000_00000000_11111111_11111111_11111111;
        protected const ulong UuidVariant = 0b_10000000_00000000_00000000_00000000;
        protected const ulong TwelveBitMask = 0b_00000000_00000000_00001111_11111111;
        protected const byte Version = 0b_0111;
        
        internal IUtcDateTimeProvider _utcDateTimeProvider;
        
        public abstract BigInteger NextId();
    }
}