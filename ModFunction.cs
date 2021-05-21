using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace DataStructure
{
    class ModFunction
    {
        public static BigInteger ModExp(BigInteger a, BigInteger b, BigInteger p)
        {
            //Get the result of (a^b) % p
            var result = new BigInteger(1);

            if (b.IsZero)
            {
                return result;
            }

            var t = a * a % p;
            if (b.IsEven)
            {
                result *= ModExp(t, b / 2, p);
                result %= p;
            }
            else
            {
                result *= a;
                result %= p;
                result *= ModExp(t, b / 2, p);
                result %= p;
            }

            return result;
        }

        public static BigInteger ModInverse(BigInteger a, BigInteger p)
        {
            return ModExp(a, p - 2, p);
        }

        public static BigInteger ModCombine(long n, long k, BigInteger p)
        {
            //Get the result of C(n,k)%p
            var result = new BigInteger(1);

            //C(n,k) = n!/k!/(n-k)!

            if (k > n / 2)
            {
                k = n - k;
            }

            var t = new BigInteger(1);
            for (long i = 1; i <= k; i++)
            {
                t *= i;
                t %= p;
            }
            var m1 = ModInverse(t, p);

            for (long i = k + 1; i <= n - k; i++)
            {
                t *= i;
                t %= p;
            }
            var m2 = ModInverse(t, p);

            for (long i = n - k + 1; i <= n; i++)
            {
                t *= i;
                t %= p;
            }
            var m3 = t;

            result *= m1;
            result %= p;
            result *= m2;
            result %= p;
            result *= m3;
            result %= p;

            return result;
        }
    }
}
