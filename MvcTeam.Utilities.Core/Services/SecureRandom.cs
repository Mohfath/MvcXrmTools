using System;
using System.Security.Cryptography;

namespace MvcTeam.Utilities.Services
{
    //Random numbers for the workflow steps. A new System.Random per call is seeded from the clock, so two steps that run
    //within the same few milliseconds get the same "random" number; this one uses the operating system's generator instead,
    //is safe to use from several workflows at once, and makes every value equally likely.
    public static class SecureRandom
    {
        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        //A number from minInclusive up to, but not including, maxExclusive
        public static int Next(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "Max must be greater than min.");

            uint range = (uint)((long)maxExclusive - minInclusive);
            //Values at or above the largest multiple of the range are skipped, so no number is favoured
            uint limit = uint.MaxValue - (uint.MaxValue % range);
            byte[] buffer = new byte[4];
            uint value;
            do
            {
                Generator.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            } while (value >= limit);

            return (int)(minInclusive + (long)(value % range));
        }
    }
}
