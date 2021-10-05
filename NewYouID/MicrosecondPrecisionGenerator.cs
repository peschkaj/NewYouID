using System;
using System.Numerics;

namespace NewYouID
{
    /// <summary>
    /// Generate microsecond precision UUIDv7 IDs based on the draft specification found at:
    /// https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01#section-4.4
    ///
    /// Field layout
    /// <code>
    ///    0                   1                   2                   3
    ///    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                            unixts                             |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |unixts |         usec          |  ver  |         usec          |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |var|             seq           |            rand               |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///   |                             rand                              |
    ///   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </code>
    ///
    /// *  The first 36 bits have been dedicated to the Unix Timestamp
    ///    (unixts)
    ///
    /// *  All 12 bits of scenario subsec_a is fully dedicated to providing
    ///    sub-second encoding for the Microsecond precision (usec).
    ///
    /// *  The 4 Version bits remain unchanged (ver).
    ///
    /// *  All 12 bits of subsec_b have been dedicated to providing sub-
    ///    second encoding for the Microsecond precision (usec).
    ///
    /// *  The 2 Variant bits remain unchanged (var).
    ///
    /// *  A 14 bit motonic clock sequence counter (seq) has been embedded in
    ///    the most significant position of subsec_seq_node
    ///
    /// *  Finally the remaining 48 bits in the subsec_seq_node section are
    ///    layout is filled out with random data to pad the length and
    ///    provide guaranteed uniqueness (rand).
    /// </summary>
    public class MicrosecondPrecisionGenerator : AbstractGenerator, IUuidGenerator
    {
        private ulong _lastUpdate;
        private ulong _seq;
        private ulong _low;

        #region ctor
        public MicrosecondPrecisionGenerator(ulong? identifier = null, IUtcDateTimeProvider utcDateTimeProvider = null)
        {
            if (!identifier.HasValue)
            {
                var buffer = new byte[sizeof(ulong)];
                var random = new Random();
                random.NextBytes(buffer);

                identifier = BitConverter.ToUInt64(buffer, 0);
            }
            
            _low = UuidVariant & (identifier.Value & Low40Mask);

            _utcDateTimeProvider = utcDateTimeProvider ?? UtcDateTimeProvider.Instance;
        }
        #endregion

        public override BigInteger NextId()
        {
            var ticks = (ulong)(_utcDateTimeProvider.UtcNow - DateTime.UnixEpoch).Ticks;

            if (ticks > _lastUpdate)
            {
                _lastUpdate = ticks;
                _seq = 0;
            }
            else
            {
                _seq++;
            }
            
            var unixts = _lastUpdate / TimeSpan.TicksPerMillisecond;
            var usec = (_lastUpdate - (unixts * TimeSpan.TicksPerMillisecond)) & MicrosecondUnixTsMask;

            var high = ((unixts & ThirtySixBitMask) << 28) 
                       | ((usec >> 12) << 16) 
                       | (Version << 12) 
                       | (usec & TwelveBitMask);

            var low = _low | (_seq << 40);
            
            var bi = new BigInteger(high);
            bi <<= 64;
            bi |= low;
            return bi;
        }
    }
}