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
}