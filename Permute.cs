using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DataStructure
{
    public class Permute
    {
        public void PrintPermute(int N, int K)
        {
            var result = GetPermute(N, K);

            foreach (var t in result)
            {
                for (int i = 0; i < K; i++)
                {
                    Console.Write("{0} ", t[i]);
                }
                Console.WriteLine();
            }
        }

        public List<int[]> GetPermute(int N, int K)
        {
            if (K == 0)
            {
                return new List<int[]>(1);
            }

            BigInteger count = new BigInteger(1);

            for (int i = 1; i <= N; i++)
            {
                count *= i;
            }
            for (int i = 1; i <= N - K; i++)
            {
                count /= i;
            }

            if (count > int.MaxValue)
            {
                return null;
            }
            var result = new List<int[]>((int)count);
            var chosen = new bool[N];
            var num = new int[K];

            GetPermute(N, K, chosen, 0, num, result);

            return result;
        }

        private void GetPermute(int N, int K, bool[] chosen, int index, int[] num, List<int[]> result)
        {
            if (index == K)
            {
                result.Add(num);
                return;
            }

            for (int i = 0; i < N; i++)
            {
                if (chosen[i])
                {
                    continue;
                }

                var nchosen = new bool[N];
                Array.Copy(chosen, nchosen, N);
                nchosen[i] = true;
                var nnum = new int[K];
                Array.Copy(num, nnum, K);
                nnum[index] = i;
                GetPermute(N, K, nchosen, index + 1, nnum, result);
            }
        }

    }

    public class Combine
    {
        public void PrintCombine(int N, int K)
        {
            var result = GetCombine(N, K);

            foreach (var t in result)
            {
                for (int i = 0; i < K; i++)
                {
                    Console.Write("{0} ", t[i]);
                }
                Console.WriteLine();
            }
        }

        public List<int[]> GetCombine(int N, int K)
        {
            if (K == 0)
            {
                return new List<int[]>(1);
            }

            BigInteger count = new BigInteger(1);

            for (int i = 1; i <= N; i++)
            {
                count *= i;
            }
            for (int i = 1; i <= K; i++)
            {
                count /= i;
            }
            for (int i = 1; i <= N - K; i++)
            {
                count /= i;
            }

            if (count > int.MaxValue)
            {
                return null;
            }
            var result = new List<int[]>((int)count);
            var chosen = new bool[N];

            GetCombine(N, K, chosen, result);

            return result;
        }

        private void GetCombine(int N, int K, bool[] chosen, List<int[]> result)
        {
            var count = 0;
            var arr = new int[K];
            var ind = 0;
            var max = 0;
            for (int i = 0; i < N; i++)
            {
                if (chosen[i])
                {
                    count++;
                    arr[ind++] = i;
                    max = i;
                }
            }

            if (count == K)
            {
                result.Add(arr);
                return;
            }

            for (int i = max; i < N; i++)
            {
                if (chosen[i])
                {
                    continue;
                }

                var nchosen = new bool[N];
                Array.Copy(chosen, nchosen, N);
                nchosen[i] = true;
                GetCombine(N, K, nchosen, result);
            }
        }

    }

    public class Select
    {
        public void PrintSelect(int N, int K)
        {
            var result = GetSelect(N, K);

            foreach (var t in result)
            {
                for (int i = 0; i < N; i++)
                {
                    Console.Write("{0} ", t[i]);
                }
                Console.WriteLine();
            }
        }

        public List<int[]> GetSelect(int N, int K)
        {
            if (K == 0)
            {
                return new List<int[]>(1);
            }

            BigInteger count = new BigInteger(1);

            for (int i = 1; i <= N; i++)
            {
                count *= K;
            }

            if (count > int.MaxValue)
            {
                return null;
            }
            var result = new List<int[]>((int)count);
            var chosen = new int[N];

            GetSelect(N, K, chosen, 0, result);

            return result;
        }

        private void GetSelect(int N, int K, int[] chosen, int index, List<int[]> result)
        {
            if (index == N)
            {
                result.Add(chosen);
                return;
            }

            for (int i = 0; i < K; i++)
            {
                var nchosen = new int[N];
                Array.Copy(chosen, nchosen, N);
                nchosen[index] = i;
                GetSelect(N, K, nchosen, index + 1, result);
            }
        }

    }
}
