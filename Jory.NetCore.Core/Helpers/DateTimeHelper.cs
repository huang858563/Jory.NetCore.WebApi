using System;

namespace Jory.NetCore.Core
{
    public class DateTimeHelper
    {
        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ConvertToLong(DateTime date)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            return (new DateTimeOffset(date).UtcTicks - startTime.Ticks) / 10000;
        }

        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return startTime.Add(new TimeSpan(timestamp * 10000));
        }
    }
}
