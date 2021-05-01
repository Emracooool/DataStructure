using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class MaxBipartiteMatching
    {
        public MaxBipartiteMatching(bool[,] graph)
        {
            this.graph = graph;
            M = graph.GetLength(0);
            N = graph.GetLength(1);
        }

        public int GetMaxBPM()
        {
            var match = new int[N];

            for (int i = 0; i < N; i++)
            {
                match[i] = -1;
            }

            var result = 0;

            for (int i = 0; i < M; i++)
            {
                var visit = new bool[N];

                if (BPM(i, visit, match))
                {
                    result++;
                }
            }

            return result;
        }

        private bool BPM(int i, bool[] visit, int[] match)
        {
            for (int j = 0; j < N; j++)
            {
                if (graph[i, j] && !visit[j])
                {
                    visit[j] = true;

                    if ((match[j] < 0) || BPM(match[j], visit, match))
                    {
                        match[j] = i;
                        return true;
                    }
                }
            }


            return false;
        }



        private bool[,] graph;
        private int M;
        private int N;
    }
}
