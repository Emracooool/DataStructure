using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class AVLTreeNodeCPC<T>
    {
        public T data;
        public int childrenNum = 0;
        public int leftHeight = 0;
        public int rightHeight = 0;
        public int weight = 1;
        public AVLTreeNodeCPC<T> parent = null;
        public AVLTreeNodeCPC<T> leftChild = null;
        public AVLTreeNodeCPC<T> rightChild = null;

        public AVLBST<T> subTree = null;
    }
}
