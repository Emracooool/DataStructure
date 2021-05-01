using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class SegmentTree<T>
    {
        public SegmentTree(T[] arr, T defValue, OpDelegate operation)
        {
            var n = arr.Length;
            var t = (int)(Math.Ceiling(Math.Log(n) / Math.Log(2)));
            var size = (1 << (t + 1)) - 1;
            data = new T[size];
            this.defValue = defValue;
            this.operation = operation;
            len = n;

            BuildTree(arr, 0, n - 1, 0);
        }

        private T BuildTree(T[] arr, int start, int end, int index)
        {
            if (start == end)
            {
                data[index] = arr[start];
                return arr[start];
            }

            var mid = (start + end) / 2;
            data[index] = operation(BuildTree(arr, start, mid, index * 2 + 1), BuildTree(arr, mid + 1, end, index * 2 + 2));

            return data[index];
        }

        public T GetCombine(int start, int end)
        {
            if (start > end)
            {
                var t = start;
                start = end;
                end = t;
            }
            return GetCombine(0, len - 1, start, end, 0);
        }

        private T GetCombine(int curStart, int curEnd, int qStart, int qEnd, int index)
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
            return operation(GetCombine(curStart, mid, qStart, qEnd, index * 2 + 1), GetCombine(mid + 1, curEnd, qStart, qEnd, index * 2 + 2));
        }

        //This function does not work if update is unequal
        public void UpdateValue(int i, int n, T diff)
        {
            UpdateValue(0, n - 1, i, diff, 0);
        }

        private void UpdateValue(int start, int end, int i, T diff, int index)
        {
            if ((start > i) || (end < i))
            {
                return;
            }

            data[index]  = operation(data[index], diff);
            if (start != end)
            {
                var mid = (start + end) / 2;
                UpdateValue(start, mid, i, diff, index * 2 + 1);
                UpdateValue(mid + 1, end, i, diff, index * 2 + 2);
            }
        }

        public delegate T OpDelegate(T a, T b);

        public T[] data;
        private int len;
        private readonly OpDelegate operation;
        private T defValue;
    }
}
