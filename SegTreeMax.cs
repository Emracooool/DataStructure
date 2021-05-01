using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class SegTreeMax
    {
        public SegTreeMax(int[] arr, int defValue)
        {
            var n = arr.Length;
            var t = (int)(Math.Ceiling(Math.Log(n) / Math.Log(2)));
            var size = (1 << (t + 1)) - 1;
            data = new int[size];
            this.defValue = defValue;
            len = n;

            BuildTree(arr, 0, n - 1, 0);
        }

        private int BuildTree(int[] arr, int start, int end, int index)
        {
            if (start == end)
            {
                data[index] = arr[start];
                return arr[start];
            }

            var mid = (start + end) / 2;
            data[index] = Math.Max(BuildTree(arr, start, mid, index * 2 + 1), BuildTree(arr, mid + 1, end, index * 2 + 2));

            return data[index];
        }

        public int GetCombine(int start, int end)
        {
            if (start > end)
            {
                var t = start;
                start = end;
                end = t;
            }
            return GetMaxValue(0, len - 1, start, end, 0);
        }

        private int GetMaxValue(int curStart, int curEnd, int qStart, int qEnd, int index)
        {
            if ((curStart >= qStart) && (curEnd <= qEnd))
            {
                return data[index];
            }

            if ((curEnd < qStart) || (curStart > qEnd))
            {
                return defValue;
            }

            var mid = (curStart + curEnd) / 2;
            return Math.Max(GetMaxValue(curStart, mid, qStart, qEnd, index * 2 + 1), GetMaxValue(mid + 1, curEnd, qStart, qEnd, index * 2 + 2));
        }

        //This function does not work if update is unequal
        public void UpdateValue(int i, int newdata)
        {
            UpdateValue(0, len - 1, i, newdata);
        }

        private void UpdateValue(int start, int end, int i, int newdata)
        {
            var index = 0;
            while (start != end)
            {
                var mid = (start + end) / 2;
                if (i > mid)
                {
                    start = mid + 1;
                    index = index * 2 + 2;
                }
                else if (i < mid)
                {
                    end = mid;
                    index = index * 2 + 1;
                }
                else
                {
                    break;
                }
            }

            data[index] = newdata;
            while (index > 0)
            {
                index = (index - 1) / 2;
                data[index] = Math.Max(data[index * 2 + 1], data[index * 2 + 2]);
            }

            if (data.Length > 1)
            {
                data[0] = Math.Max(data[1], data[2]);
            }
        }

        public int[] data;
        private int len;
        private int defValue;
    }
}
