using System;

namespace CryptoSharp.Proofs
{
    public class SimpleProof
    {
        public static void Prove()
        {
            var baseG = 2;
            var x = 5;
            var y = Math.Pow(baseG, x);
            var q = 17;
            var v = GetRandomValue(q);

        }

        public static double GetRandomValue(int q)
        {
            var randomValue = Math.Pow(3, 4) % q;
            return randomValue;
            //var randomizer = new Random();
            //randomizer.Next()

        }
    }
}