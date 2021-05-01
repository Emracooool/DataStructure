using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class AVLTreeNode<T>
    {
        public T data;
        public int childrenNum = 0;
        public int leftHeight = 0;
        public int rightHeight = 0;
        public int weight = 1;
        public AVLTreeNode<T> parent = null;
        public AVLTreeNode<T> leftChild = null;
        public AVLTreeNode<T> rightChild = null;
    }
}
