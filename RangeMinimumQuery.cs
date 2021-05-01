using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class RangeMinimumQuery
    {
        //Sparse Table
        public RangeMinimumQuery(int[] arr)
        {
            len = arr.Length;
            BuildData(arr);
        }

        private void BuildData(int[] arr)
        {
            var L = (int)Math.Floor(Math.Log(len) / Math.Log(2)) + 1;

            data = new int[len, L];

            for (int i = 0; i < len; i++)
            {
                data[i, 0] = arr[i];
            }

            for (int j = 1; j < L; j++)
            {
                for (int i = 0; i < len; i++)
                {
                    if (i + (1 << (j - 1)) < len)
                    {
                        data[i, j] = Math.Min(data[i, j - 1], data[i + (1 << (j - 1)), j - 1]);
                    }
                    else
                    {
                        data[i, j] = data[i, j - 1];
                    }
                }
            }
        }

        public int GetMin(int start, int end)
        {
            var L = (int)Math.Floor(Math.Log(end - start + 1) / Math.Log(2));
            return Math.Min(data[start, L], data[end - (1 << L) + 1, L]);
        }

        private int[,] data;
        private int len;
    }
}
