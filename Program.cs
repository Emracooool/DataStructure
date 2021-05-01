using System;
using System.Collections.Generic;

namespace DataStructure
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Test();
        }


        public void Test()
        {
            try
            {
                var p = new Permute();
                p.PrintPermute(8, 3);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void GetMaxContinuousArea()
        {
            var graph = new bool[3, 3]
                {
                    { false, false, false},
                    { false, false, false},
                    { false, false, false},
                };

            var g = new MatrixFunction();
            Console.WriteLine(g.GetMaxContinousArea(graph));
        }

        private void GetMatching()
        {
            var graph = new bool[6, 6]
                {
                    { false, true, true, false, false, false},
                    { false, false, false, false, false, false},
                    { true, false, false, true, false, false},
                    { false, false, true, false, false, false},
                    { false, false, true, true, false, false},
                    { false, false, false, false, false, true},
                };

            var g = new MaxBipartiteMatching(graph);
            Console.WriteLine(g.GetMaxBPM());
        }

        private void RangeMinQuery()
        {
            try
            {

                var arr = new int[] { 21, 2, 33, 24, 15, 36, 27, 8, 9, 10, 11 };
                var rmq = new RangeMinimumQuery(arr);
                Console.WriteLine(rmq.GetMin(5, 5));
                Console.WriteLine(rmq.GetMin(3, 9));
                Console.WriteLine(rmq.GetMin(0, 10));
                rmq.GetMin(3, 9);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    public class Coordinate
    {
        public Coordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int x;
        public int y;
        public int z;
    }
}
