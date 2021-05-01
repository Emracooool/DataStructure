using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class MatrixFunction
    {
        public MatrixFunction()
        {

        }

        public int GetMaxContinousArea(bool[,] matrix)
        {
            var M = matrix.GetLength(0);
            var N = matrix.GetLength(1);

            var select = new bool[M, N];
            var max = 0;
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (matrix[i, j] && !select[i, j])
                    {
                        select[i, j] = true;
                        var count = 1 + GetContinousArea(matrix, M, N, i, j, select);
                        if (count > max)
                        {
                            max = count;
                        }
                    }
                }
            }

            return max;
        }

        private int GetContinousArea(bool[,] matrix, int M, int N, int i, int j, bool[,] select)
        {
            var count = 0;
            if ((i > 0) && matrix[i - 1, j] && !select[i - 1, j])
            {
                select[i - 1, j] = true;
                count++;
                count += GetContinousArea(matrix, M, N, i - 1, j, select);
            }
            if ((i < M - 1) && matrix[i + 1, j] && !select[i + 1, j])
            {
                select[i + 1, j] = true;
                count++;
                count += GetContinousArea(matrix, M, N, i + 1, j, select);
            }
            if ((j > 0) && matrix[i, j - 1] && !select[i, j - 1])
            {
                select[i, j - 1] = true;
                count++;
                count += GetContinousArea(matrix, M, N, i, j - 1, select);
            }
            if ((j < N - 1) && matrix[i, j + 1] && !select[i, j + 1])
            {
                select[i, j + 1] = true;
                count++;
                count += GetContinousArea(matrix, M, N, i, j + 1, select);
            }

            return count;
        }

    }

}
