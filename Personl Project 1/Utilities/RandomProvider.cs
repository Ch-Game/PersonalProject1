using System.Security.Cryptography;

namespace PersonalProject1.Utilities
{
    public static class RandomProvider
    {
        public static int GetInt(int minInclusive, int maxExclusive)
        {
            return RandomNumberGenerator.GetInt32(minInclusive, maxExclusive);
        }
    }
}
