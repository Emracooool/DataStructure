using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class SegTreeAdd
    {
        public SegTreeAdd(int[] arr, int defValue)
        {
            var n = arr.Length;
            var t = (int)(Math.Ceiling(Math.Log(n) / Math.Log(2)));
            var size = (1 << (t + 1)) - 1;
            data = new int[size];
            lazy = new int[size];
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
            data[index] = BuildTree(arr, start, mid, index * 2 + 1) + BuildTree(arr, mid + 1, end, index * 2 + 2);

            return data[index];
        }

        public int GetSum(int start, int end)
        {
            if (start > end)
            {
                var t = start;
                start = end;
                end = t;
            }
            return GetSum(0, len - 1, start, end, 0);
        }

        private int GetSum(int curStart, int curEnd, int qStart, int qEnd, int index)
        {
            if (lazy[index] != 0)
            {
                data[index] += (curEnd - curStart + 1) * lazy[index];

                if (curStart != curEnd)
                {
                    lazy[index * 2 + 1] += lazy[index];
                    lazy[index * 2 + 2] += lazy[index];
                }
                lazy[index] = 0;
            }

            if ((curStart >= qStart) && (curEnd <= qEnd))
            {
                return data[index];
            }

            if ((curEnd < qStart) || (curStart > qEnd))
            {
                return defValue;
            }

            var mid = (curStart + curEnd) / 2;
            return GetSum(curStart, mid, qStart, qEnd, index * 2 + 1) + GetSum(mid + 1, curEnd, qStart, qEnd, index * 2 + 2);
        }

        public void UpdateValue(int i, int diff)
        {
            UpdateValue(0, len - 1, i, diff, 0);
        }

        private void UpdateValue(int start, int end, int i, int diff, int index)
        {
            if ((start > i) || (end < i))
            {
                return;
            }

            data[index] = data[index] + diff;
            if (start != end)
            {
                var mid = (start + end) / 2;
                UpdateValue(start, mid, i, diff, index * 2 + 1);
                UpdateValue(mid + 1, end, i, diff, index * 2 + 2);
            }
        }

        public void UpdateValueRange(int start, int end, int diff)
        {
            UpdateValueRange(start, end, 0, len - 1, diff, 0);
        }

        private void UpdateValueRange(int us, int ue, int curStart, int curEnd, int diff, int index)
        {
            if (lazy[index] != 0)
            {
                data[index] += (curEnd - curStart + 1) * lazy[index];

                if (curStart != curEnd)
                {
                    lazy[index * 2 + 1] += lazy[index];
                    lazy[index * 2 + 2] += lazy[index];
                }
                lazy[index] = 0;
            }

            if ((curStart > curEnd) || (us > curEnd) || (ue < curStart))
            {
                return;
            }

            //fully in range
            if ((us <= curStart) && (ue >= curEnd))
            {
                data[index] += (curEnd - curStart + 1) * diff;
                if (curStart != curEnd)
                {
                    lazy[index * 2 + 1] += diff;
                    lazy[index * 2 + 2] += diff;
                }
                return;
            }

            var mid = (curStart + curEnd) / 2;
            UpdateValueRange(us, ue, curStart, mid, diff, index * 2 + 1);
            UpdateValueRange(us, ue, mid + 1, curEnd, diff, index * 2 + 2);
            data[index] = data[index * 2 + 1] + data[index * 2 + 2];
        }

        public delegate int OpDelegate(int a, int b);

        public int[] data;
        public int[] lazy;
        private int len;
        private int defValue;
    }
}
