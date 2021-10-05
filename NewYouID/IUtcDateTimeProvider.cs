using System;
using Microsoft.VisualBasic;

namespace NewYouID
{
    public interface IUtcDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public static IUtcDateTimeProvider Instance { get; }
    }

    public class UtcDateTimeProvider : IUtcDateTimeProvider
    {
        public static IUtcDateTimeProvider Instance { get; } = new UtcDateTimeProvider();
    }
}