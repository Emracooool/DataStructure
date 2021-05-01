using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class RangeTreeNode<T>
    {
        public T data;
        public int leafNumber = 0;
        public int weight = 1;
        public T max;
        public RangeTreeNode<T> parent = null;
        public RangeTreeNode<T> leftChild = null;
        public RangeTreeNode<T> rightChild = null;
    }
}
