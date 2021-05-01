using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class RangeTreeNode3D<T>
    {
        public T data;
        public int leafNumber = 0;
        public int weight = 1;
        public T max;
        public RangeTreeNode3D<T> parent = null;
        public RangeTreeNode3D<T> leftChild = null;
        public RangeTreeNode3D<T> rightChild = null;

        public RangeTree2D<T> subTree = null;
        public List<T> leafData;//Only used in construction
    }
}
