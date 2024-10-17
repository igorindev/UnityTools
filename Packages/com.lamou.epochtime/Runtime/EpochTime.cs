using System;

namespace EpochTime
{
    public static class EpochTime
    {
        public static DateTime UtcNow => EpochDebugTimeSubsystem.GetUtcTime();
    }
}
