using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class _2DAVLTreeNode<T>
    {
        public T data;
        public int childrenNum = 0;
        public int leftHeight = 0;
        public int rightHeight = 0;
        public int weight = 1;
        public _2DAVLTreeNode<T> parent = null;
        public _2DAVLTreeNode<T> leftChild = null;
        public _2DAVLTreeNode<T> rightChild = null;

        //Nodes with the same primary value
        public AVLBST<T> nodeTree = null;

        //Nodes with the same primary value and all children
        public AVLBST<T> subTree = null;
    }
}
