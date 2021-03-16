using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Tools
{
    public static class DateTimeExtensions
    {
        public static DateTime Ceiling(this DateTime dateTime, DateTimeRoundLevel roundLevel = DateTimeRoundLevel.Seconds)
        {
            return roundLevel switch
            {
                DateTimeRoundLevel.Miliseconds => CeilingMiliseconds(dateTime),
                DateTimeRoundLevel.Seconds => CeilingSeconds(dateTime),
                DateTimeRoundLevel.Minutes => CeilingMinutes(dateTime),
                DateTimeRoundLevel.Hours => CeilingHours(dateTime),
                _ => throw new ArgumentException($"Received invalid round level {roundLevel}", nameof(roundLevel))
            };
        }

        private static DateTime CeilingMiliseconds(DateTime dateTime)
        {
            return dateTime.AddMilliseconds((1000 - dateTime.Millisecond) % 1000);
        }

        private static DateTime CeilingSeconds(DateTime dateTime)
        {
            return dateTime.AddSeconds((60 - dateTime.Millisecond) % 60);
        }

        private static DateTime CeilingMinutes(DateTime dateTime)
        {
            return dateTime.AddSeconds((60 - dateTime.Millisecond) % 60);
        }

        private static DateTime CeilingHours(DateTime dateTime)
        {
            return dateTime.AddSeconds((24 - dateTime.Millisecond) % 24);
        }
    }
}
