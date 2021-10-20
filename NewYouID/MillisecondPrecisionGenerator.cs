using System;
using System.Numerics;

namespace NewYouID
{
    /// <summary>
    /// Generate millisecond precision UUIDv7 IDs based on the draft specification found at:
    /// https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01#section-4.4
    ///
    /// Field Layout:
    /// <code>
    ///     0                   1                   2                   3
    ///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |                            unixts                             |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |unixts |         msec          |  ver  |          seq          |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |var|                   subsec_seq_node                         |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///    |                       subsec_seq_node                         |
    ///    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </code>
    ///
    ///    *  The first 36 bits have been dedicated to the Unix Timestamp
    ///      (unixts)
    ///
    ///   *  All 12 bits of scenario subsec_a is fully dedicated to millisecond
    ///      information (msec).
    ///
    ///   *  The 4 Version bits remain unchanged (ver).
    ///
    ///   *  All 12 bits of subsec_b have been dedicated to a motonic clock
    ///      sequence counter (seq).
    ///
    ///   *  The 2 Variant bits remain unchanged (var).
    ///
    ///   *  Finally the remaining 62 bits in the subsec_seq_node section are
    ///      layout is filled out with random data to pad the length and
    ///      provide guaranteed uniqueness (rand).
    /// </summary>
    public class MillisecondPrecisionGenerator : AbstractGenerator, IUuidGenerator
    {
        private ulong _lastUpdateInMs;
        private ulong _seq;
        private readonly ulong _low;

        #region ctor
        public MillisecondPrecisionGenerator(ulong? identifier = null, IUtcDateTimeProvider utcDateTimeProvider = null)
        {
            if (!identifier.HasValue)
            {
                var buffer = new byte[sizeof(ulong)];
                var random = new Random();
                random.NextBytes(buffer);
                
                identifier = BitConverter.ToUInt64(buffer, 0);
            }
            // Only need 62-bits
            _low = (UuidVariant << UuidVariantShift)  | (identifier.Value & Low62Mask);
            
            _utcDateTimeProvider = utcDateTimeProvider ?? UtcDateTimeProvider.Instance;
        }
        #endregion

        public override BigInteger NextId()
        {
            var currentDiffInMs = (ulong)(_utcDateTimeProvider.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;

            if (currentDiffInMs > _lastUpdateInMs)
            {
                _lastUpdateInMs = currentDiffInMs;
                _seq = 0;
            }
            else
            {
                _seq++;
            }

            var unixts = (ulong)(_lastUpdateInMs * 0.001);
            var msec = (ulong)(_lastUpdateInMs - (unixts * 1000));
            
            var high = ((unixts & MillisecondUnixTsMask) << 28) 
                       | ((msec & TwelveBitMask) << 16) 
                       | (Version << 12) 
                       | (_seq & TwelveBitMask);

            var bi = new BigInteger(high);
            bi <<= 64;
            bi |= _low;
            return bi;
        }
    }
}