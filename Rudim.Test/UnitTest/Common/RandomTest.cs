using Rudim.Common;
using System.Collections.Generic;
using Xunit;

namespace Rudim.Test.UnitTest.Common
{
    public class RandomTest
    {
        [Fact]
        public void ShouldGenerateUniqueULongNumbers()
        {
            HashSet<ulong> generatedNumbers = [];
            for (int i = 0; i < 500; i++)
            {
                ulong number = Random.NextULong();
                Assert.True(generatedNumbers.Add(number), $"Collision detected for ulong number: {number}");
            }
        }

        [Fact]
        public void ShouldGenerateUniqueIntNumbers()
        {
            HashSet<int> generatedNumbers = [];
            for (int i = 0; i < 500; i++)
            {
                int number = Random.NextInt();
                Assert.True(generatedNumbers.Add(number), $"Collision detected for int number: {number}");
            }
        }
    }
}