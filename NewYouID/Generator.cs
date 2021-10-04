using System;
using System.Numerics;

namespace NewYouID
{
    /// <summary>
    /// Generate UUIDv7 IDs based on the draft specification found at:
    /// https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01#section-4.4
    ///
    /// Field Layout:
    /// <code>
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |                            unixts                             |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |unixts |       subsec_a        |  ver  |       subsec_b        |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |var|                   subsec_seq_node                         |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |                       subsec_seq_node                         |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </code>
    ///
    ///    unixts:
    ///    36-bit big-endian unsigned Unix Timestamp value
    ///
    ///    subsec_a:
    ///    12-bits allocated to sub-section precision values.
    ///
    ///    ver:
    ///    The 4 bit UUIDv8 version (0111)
    ///
    ///    subsec_b:
    ///    12-bits allocated to sub-section precision values.
    ///
    ///    var:
    ///    2-bit UUID variant (10)
    ///
    ///    subsec_seq_node:
    ///    The remaining 62 bits which MAY be allocated to any combination of
    ///    additional sub-section precision, sequence counter, or pseudo-
    ///    random data.
    /// </summary>
    public class Generator
    {
        private const ulong Low62Mask = 0b_00111111_11111111_11111111_11111111;
        private const ulong UuidVariant   = 0b_10000000_00000000_00000000_00000000;

        private const ulong CounterMask = 0b_00000000_00000000_00001111_11111111;
        // _counter is constrained to a 12-bit number by CounterMask
        private ulong _counter = 0;

        private const byte Version = 0b_0111;
        // _low contains both `var` and `subsec_seq_node`
        private readonly UInt64 _low;

        private ulong _lastUpdateInMs = 0;

        public Generator()
        {
            var buffer = new byte[sizeof(ulong)];
            var random = new Random();
            random.NextBytes(buffer);
            // Only need 62-bits
            _low = UuidVariant & (BitConverter.ToUInt64(buffer, 0) & Low62Mask);
        }

        public Generator(ulong identifier)
        {
            // Only need 62-bits
            _low = UuidVariant & (identifier & Low62Mask);
        }

        public BigInteger NextID()
        {
            var currentDiffInMs = (ulong)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

            if (currentDiffInMs > _lastUpdateInMs)
            {
                _lastUpdateInMs = currentDiffInMs;
                _counter = 0;
            }
            else
            {
                _counter++;
            }

            var unixts = (ulong)(_lastUpdateInMs * 0.001);
            var subsec_a = (ulong)(_lastUpdateInMs - (unixts * 1000));
            
            // TODO add masks for all of these values
            ulong high = (unixts << 28) | (subsec_a << 16) | (Version << 12) | (_counter & CounterMask);

            var bi = new BigInteger(high);
            bi <<= 64;
            bi |= _low;
            return bi;
        }
    }
}