using System;
using System.Globalization;
using NewYouID;

namespace NewYouIDTests
{
    public class FakeUtcDateTimeProvider : IUtcDateTimeProvider
    {
        CultureInfo provider = CultureInfo.InvariantCulture;
        private string format = "ddd dd MMM yyyy h:mm tt zzz";
        
        public DateTime UtcNow => DateTime.ParseExact("Sun 15 Jun 2008 8:30 AM -06:00", format, provider);
        public static IUtcDateTimeProvider Instance { get; } = new FakeUtcDateTimeProvider();
    }
}