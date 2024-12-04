﻿namespace Rudim.Common
{
    /*
     * This class is to generate fast random numbers using
     * https://en.wikipedia.org/wiki/Xorshift
     */
    internal static class Random
    {
        // Arbitrary starting seed
        private static ulong _ulongState = 1804289383;
        private static int _intState = 1804289383;

        public static ulong NextULong()
        {
            var randomNumber = _ulongState;
            randomNumber ^= randomNumber << 13;
            randomNumber ^= randomNumber >> 7;
            randomNumber ^= randomNumber << 17;
            return _ulongState = randomNumber;
        }

        public static int NextInt()
        {
            var randomNumber = _intState;
            randomNumber ^= randomNumber << 13;
            randomNumber ^= randomNumber >> 17;
            randomNumber ^= randomNumber << 5;
            return _intState = randomNumber;
        }
    }
}