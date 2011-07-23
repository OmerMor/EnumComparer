using System;
using System.Collections.Generic;

namespace SitraUtils.Benchmarks
{
    internal class DayOfWeekComparer : IEqualityComparer<DayOfWeek>
    {
        public bool Equals(DayOfWeek x, DayOfWeek y)
        {
            return x == y;
        }

        public int GetHashCode(DayOfWeek obj)
        {
            return (int)obj;
        }
    }

    public enum DayOfWeek : uint
    {
        Sunday = System.DayOfWeek.Sunday,
        Monday = System.DayOfWeek.Monday,
        Tuesday = System.DayOfWeek.Tuesday,
        Wednesday = System.DayOfWeek.Wednesday,
        Thrusday = System.DayOfWeek.Thursday,
        Friday = System.DayOfWeek.Friday,
        Saturday = System.DayOfWeek.Saturday,
    }
}