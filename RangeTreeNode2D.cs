using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class RangeTreeNode2D<T>
    {
        public T data;
        public int leafNumber = 0;
        public int weight = 1;
        public T max;
        public RangeTreeNode2D<T> parent = null;
        public RangeTreeNode2D<T> leftChild = null;
        public RangeTreeNode2D<T> rightChild = null;

        public RangeTree<T> subTree = null;
        public List<T> leafData;//Only used in construction
    }
}
