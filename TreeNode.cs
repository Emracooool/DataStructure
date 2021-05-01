using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class TreeNode<T>
    {
        public T data;
        public int childrenNum = 0;
        public TreeNode<T> parent = null;
        public TreeNode<T> leftChild = null;
        public TreeNode<T> rightChild = null;
    }
}
