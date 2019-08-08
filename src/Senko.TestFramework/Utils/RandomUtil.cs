using System;

namespace Senko.TestFramework
{
    public static class RandomUtil
    {
        private static readonly Random Random = new Random();

        public static ulong RandomId()
        {
            return RandomULong(10000000000000000u, 99999999999999999u);
        }

        public static ulong RandomULong(ulong minValue, ulong maxValue)
        {
            return (ulong)(Random.NextDouble() * (maxValue - minValue)) + minValue;
        }
    }
}
