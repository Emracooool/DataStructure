using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class _2DAVLTree<T>
    {

        //Incomplete due to worst case is O(N)
        public _2DAVLTree(Comparison<T> primaryComparison, Comparison<T> secondaryComparison)
        {
            this.primaryComparison = primaryComparison;
            this.secondaryComparison = secondaryComparison;
        }

        //O(LgNlgN)
        public bool InsertNode(T data)
        {
            try
            {
                var newNode = new _2DAVLTreeNode<T>()
                {
                    data = data,
                    nodeTree = new AVLBST<T>(secondaryComparison),
                    subTree = new AVLBST<T>(secondaryComparison)
                };

                if (root == null)
                {
                    root = newNode;
                    return true;
                }

                var currentNode = root;
                while (true)
                {
                    var compareResult = CompareNode(data, currentNode.data);
                    if (compareResult > 0)
                    {
                        currentNode.childrenNum++;
                        if (currentNode.rightChild == null)
                        {
                            currentNode.rightChild = newNode;
                            newNode.parent = currentNode;
                            newNode.nodeTree.InsertNode(data);
                            newNode.subTree.InsertNode(data);
                            AddNewNodeForParents(currentNode, data);
                            break;
                        }
                        else
                        {
                            currentNode = currentNode.rightChild;
                        }
                    }
                    else if (compareResult < 0)
                    {
                        currentNode.childrenNum++;
                        if (currentNode.leftChild == null)
                        {
                            currentNode.leftChild = newNode;
                            newNode.parent = currentNode;
                            newNode.nodeTree.InsertNode(data);
                            newNode.subTree.InsertNode(data);
                            AddNewNodeForParents(currentNode, data);
                            break;
                        }
                        else
                        {
                            currentNode = currentNode.leftChild;
                        }
                    }
                    else
                    {
                        currentNode.weight++;
                        AddNewNodeForParents(currentNode, data);
                        return true;
                    }
                }
                BalanceNode(newNode);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        private void AddNewNodeForParents(_2DAVLTreeNode<T> currentNode, T data)
        {
            while (currentNode != null)
            {
                currentNode.subTree.InsertNode(data);
                currentNode = currentNode.parent;
            }
        }

        //O(LgNLgN)
        public bool RemoveNode(T data)
        {
            if (root == null)
            {
                return false;
            }

            var currentNode = root;
            var isLeft = true;
            while (true)
            {
                var compareResult = CompareNode(data, currentNode.data);
                if (compareResult > 0)
                {
                    isLeft = false;
                    if (currentNode.rightChild == null)
                    {
                        return false;
                    }
                    else
                    {
                        currentNode = currentNode.rightChild;
                    }
                }
                else if (compareResult < 0)
                {
                    isLeft = true;
                    if (currentNode.leftChild == null)
                    {
                        return false;
                    }
                    else
                    {
                        currentNode = currentNode.leftChild;
                    }
                }
                else
                {
                    if (currentNode.weight > 1)
                    {
                        currentNode.weight--;
                        currentNode.nodeTree.RemoveNode(data);
                        currentNode.subTree.RemoveNode(data);
                        MinusParentChildrenNum(currentNode, data);
                        return true;
                    }

                    if (currentNode.leftChild == null)
                    {
                        var parent = currentNode.parent;
                        if (currentNode.rightChild != null)
                        {
                            currentNode.rightChild.parent = parent;
                        }

                        if (parent == null)
                        {
                            //root node
                            root = currentNode.rightChild;
                        }
                        else
                        {
                            if (isLeft)
                            {
                                parent.leftChild = currentNode.rightChild;
                            }
                            else
                            {
                                parent.rightChild = currentNode.rightChild;
                            }
                        }
                        MinusParentChildrenNum(currentNode, data);
                        BalanceNode(currentNode.parent);
                        currentNode = null;//Memory Leak??
                    }
                    else if (currentNode.rightChild == null)
                    {
                        var parent = currentNode.parent;
                        if (currentNode.leftChild != null)
                        {
                            currentNode.leftChild.parent = parent;
                        }

                        if (parent == null)
                        {
                            //root node
                            root = currentNode.leftChild;
                        }
                        else
                        {
                            if (isLeft)
                            {
                                parent.leftChild = currentNode.leftChild;
                            }
                            else
                            {
                                parent.rightChild = currentNode.leftChild;
                            }
                        }
                        MinusParentChildrenNum(currentNode, data);
                        BalanceNode(currentNode.parent);
                        currentNode = null;
                    }
                    else
                    {
                        //Get the next node in InOrderTraverse
                        var nextNode = GetNextInOrderNode(currentNode);

                        if (nextNode.parent == currentNode)
                        {
                            if (currentNode.parent == null)
                            {
                                //root node
                                root = nextNode;
                                nextNode.parent = null;
                            }
                            else
                            {
                                var parent = currentNode.parent;
                                nextNode.parent = parent;

                                if (isLeft)
                                {
                                    parent.leftChild = nextNode;
                                }
                                else
                                {
                                    parent.rightChild = nextNode;
                                }
                            }
                            nextNode.leftChild = currentNode.leftChild;
                            if (currentNode.leftChild != null)
                            {
                                currentNode.leftChild.parent = nextNode;
                                nextNode.childrenNum += currentNode.leftChild.childrenNum + currentNode.leftChild.weight;
                            }

                            MinusParentChildrenNum(nextNode, data);
                            BalanceNode(nextNode);
                            currentNode = null;
                        }
                        else
                        {
                            //Copy the value of nextNode to currentNode, then delete nextNode
                            currentNode.data = nextNode.data;
                            currentNode.weight = nextNode.weight;
                            currentNode.nodeTree = nextNode.nodeTree;
                            //subtree no change
                            nextNode.parent.leftChild = nextNode.rightChild;
                            if (nextNode.rightChild != null)
                            {
                                nextNode.rightChild.parent = nextNode.parent;
                            }
                            MinusParentChildrenNum(nextNode, currentNode, nextNode.weight, data);
                            UpdateChildrenNumberByDirectChildren(currentNode);
                            BalanceNode(nextNode.parent);
                            nextNode = null;
                        }
                    }
                    return true;
                }
            }
        }

        private void MinusParentChildrenNum(_2DAVLTreeNode<T> node, T data)
        {
            var parent = node.parent;
            while (parent != null)
            {
                parent.childrenNum--;
                parent.subTree.RemoveNode(data);
                parent = parent.parent;
            }
        }

        private void MinusParentChildrenNum(_2DAVLTreeNode<T> node, _2DAVLTreeNode<T> endNode, int weight, T data)
        {
            var parent = node.parent;
            bool hasReachedEndNode = false;
            while (parent != null)
            {
                if (parent == endNode)
                {
                    hasReachedEndNode = true;
                }
                if (hasReachedEndNode)
                {
                    parent.childrenNum--;
                    parent.subTree.RemoveNode(data);
                }
                else
                {
                    parent.childrenNum -= weight;
                    //TODO: Simply
                    for (int i = 0; i < weight; i++)
                    {
                        parent.subTree.RemoveNode(node.data);
                    }
                }
                parent = parent.parent;
            }
        }

        private _2DAVLTreeNode<T> GetNextInOrderNode(_2DAVLTreeNode<T> node)
        {
            if (node.rightChild != null)
            {
                var currentNode = node.rightChild;
                while (true)
                {
                    if (currentNode.leftChild != null)
                    {
                        currentNode = currentNode.leftChild;
                    }
                    else
                    {
                        return currentNode;
                    }
                }
            }

            return null;
        }

        private void BalanceNode(_2DAVLTreeNode<T> newNode)
        {
            if (newNode == null)
            {
                return;
            }
            var currentNode = newNode;
            var parent = newNode;
            var isChildLeft = true;
            var isGrandchildLeft = true;

            while (parent != null)
            {
                if (parent.leftChild == currentNode)
                {//Left Child
                    parent.leftHeight = Math.Max(currentNode.leftHeight, currentNode.rightHeight) + 1;
                }
                else if (parent.rightChild == currentNode)
                {//Right Child
                    parent.rightHeight = Math.Max(currentNode.leftHeight, currentNode.rightHeight) + 1;
                }
                else
                {//First time
                    if (parent.leftChild != null)
                    {
                        parent.leftHeight = Math.Max(parent.leftChild.leftHeight, parent.leftChild.rightHeight) + 1;
                    }
                    else
                    {
                        parent.leftHeight = 0;
                    }
                    if (parent.rightChild != null)
                    {
                        parent.rightHeight = Math.Max(parent.rightChild.leftHeight, parent.rightChild.rightHeight) + 1;
                    }
                    else
                    {
                        parent.rightHeight = 0;
                    }
                }

                var diff = parent.leftHeight - parent.rightHeight;
                if (diff >= 2 || diff <= -2)
                {
                    if (parent.leftHeight > parent.rightHeight)
                    {
                        isChildLeft = true;
                        //The balance of sub tree may be 0 on a deletion. Do not double-rotate in this case.
                        if (parent.leftChild.leftHeight >= parent.leftChild.rightHeight)
                        {
                            isGrandchildLeft = true;
                        }
                        else
                        {
                            isGrandchildLeft = false;
                        }
                    }
                    else
                    {
                        isChildLeft = false;
                        if (parent.rightChild.leftHeight > parent.rightChild.rightHeight)
                        {
                            isGrandchildLeft = true;
                        }
                        else
                        {
                            isGrandchildLeft = false;
                        }
                    }
                    Rotate(parent, isChildLeft, isGrandchildLeft);
                }

                currentNode = parent;
                parent = parent.parent;
            }
        }

        private void Rotate(_2DAVLTreeNode<T> parent, bool isChildLeft, bool isGrandchildLeft)
        {
            try
            {
                if (isChildLeft && isGrandchildLeft)
                {
                    RightRotate(parent);
                }
                else if (isChildLeft && !isGrandchildLeft)
                {
                    LeftRightRotate(parent);
                }
                else if (!isChildLeft && isGrandchildLeft)
                {
                    RightLeftRotate(parent);
                }
                else if (!isChildLeft && !isGrandchildLeft)
                {
                    LeftRotate(parent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void LeftRotate(_2DAVLTreeNode<T> parent)
        {
            //parent must have a right child
            var newParent = parent.rightChild;

            //1. Update each node's parent
            newParent.parent = parent.parent;
            parent.parent = newParent;
            if (parent.rightChild.leftChild != null)
            {
                parent.rightChild.leftChild.parent = parent;
            }

            //2. Update each node's child
            parent.rightChild = parent.rightChild.leftChild;
            newParent.leftChild = parent;

            //3. Update height
            if (parent.rightChild != null)
            {
                parent.rightHeight = Math.Max(parent.rightChild.leftHeight, parent.rightChild.rightHeight) + 1;
            }
            else
            {
                parent.rightHeight = 0;
            }
            newParent.leftHeight = Math.Max(parent.leftHeight, parent.rightHeight) + 1;

            //4. Update children number
            UpdateChildrenNumberByDirectChildren(parent);
            UpdateChildrenNumberByDirectChildren(newParent);

            //5. Update new parent's parent
            if (newParent.parent == null)
            {
                root = newParent;
            }
            else if (newParent.parent.leftChild == parent)
            {
                newParent.parent.leftChild = newParent;
            }
            else
            {
                newParent.parent.rightChild = newParent;
            }
        }

        private void LeftRightRotate(_2DAVLTreeNode<T> parent)
        {
            //parent must have a left child and a left-right grandchild
            LeftRotate(parent.leftChild);
            RightRotate(parent);
        }

        private void RightRotate(_2DAVLTreeNode<T> parent)
        {
            //parent must have a left child
            var newParent = parent.leftChild;

            //1. Update each node's parent
            newParent.parent = parent.parent;
            parent.parent = newParent;
            if (parent.leftChild.rightChild != null)
            {
                parent.leftChild.rightChild.parent = parent;
            }

            //2. Update each node's child
            parent.leftChild = parent.leftChild.rightChild;
            newParent.rightChild = parent;

            //3. Update height
            if (parent.leftChild != null)
            {
                parent.leftHeight = Math.Max(parent.leftChild.leftHeight, parent.leftChild.rightHeight) + 1;
            }
            else
            {
                parent.leftHeight = 0;
            }
            newParent.rightHeight = Math.Max(parent.leftHeight, parent.rightHeight) + 1;

            //4. Update children number
            UpdateChildrenNumberByDirectChildren(parent);
            UpdateChildrenNumberByDirectChildren(newParent);

            //5. Update new parent's parent
            if (newParent.parent == null)
            {
                root = newParent;
            }
            else if (newParent.parent.leftChild == parent)
            {
                newParent.parent.leftChild = newParent;
            }
            else
            {
                newParent.parent.rightChild = newParent;
            }
        }

        private void RightLeftRotate(_2DAVLTreeNode<T> parent)
        {
            //parent must have a right child and a right-left grandchild
            RightRotate(parent.rightChild);
            LeftRotate(parent);
        }

        private void UpdateChildrenNumberByDirectChildren(_2DAVLTreeNode<T> parent)
        {
            parent.childrenNum = 0;
            if (parent.leftChild != null)
            {
                parent.childrenNum += parent.leftChild.childrenNum + parent.leftChild.weight;
            }
            if (parent.rightChild != null)
            {
                parent.childrenNum += parent.rightChild.childrenNum + parent.rightChild.weight;
            }
        }

        private int CompareNode(T a, T b)
        {
            return primaryComparison.Invoke(a, b);
        }

        //O(N)
        public bool CheckChildrenNum()
        {
            var result = CheckChildrenNum(root);
            if (result == -1)
            {
                Console.WriteLine("Children number check failed!");
                return false;
            }

            Console.WriteLine("Children number check success!");
            return true;
        }

        private int CheckChildrenNum(_2DAVLTreeNode<T> node)
        {
            var currentNode = node;
            while (currentNode != null)
            {
                var left = CheckChildrenNum(currentNode.leftChild);
                var right = CheckChildrenNum(currentNode.rightChild);
                if (left + right == currentNode.childrenNum)
                {
                    return currentNode.childrenNum + currentNode.weight;
                }
                else
                {
                    Console.WriteLine("Node {0} has wrong children number", currentNode.data);
                    return -1;
                }
            }
            return 0;
        }

        //O(LogN)
        public long GetSmallerNodeNumber(T node)
        {
            return GetSmallerEqualLargerNodeNumber(node)[0];
        }

        //O(LogN)
        public List<long> GetSmallerEqualLargerNodeNumber(T node)
        {
            //Find the number of nodes which is smaller than node, given that there is exactly one node that is equal
            //First, find the node
            var curNode = root;
            if (curNode == null)
            {
                return new List<long>(3) { 0, 0, 0 };
            }

            var found = false;
            var path = new List<bool>();
            while (curNode != null)
            {
                var compare = CompareNode(node, curNode.data);
                if (compare > 0)
                {
                    curNode = curNode.rightChild;
                    path.Add(false);
                }
                else if (compare < 0)
                {
                    curNode = curNode.leftChild;
                    path.Add(true);
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("Unable to find node");
                return new List<long>(3) { 0, 0, 0 };
            }

            //Travel above
            path.Reverse();
            var targetNode = curNode;
            long smallerNumber = 0;
            long equalNumber = curNode.weight;
            if (curNode.leftChild != null)
            {
                smallerNumber += curNode.leftChild.childrenNum + curNode.leftChild.weight;
            }

            foreach (var p in path)
            {
                curNode = curNode.parent;
                //If curNode is the right child of its parent, then parent and all children of parent's leftchild are smaller than targetNode
                if (!p)
                {
                    smallerNumber += curNode.weight;
                    if (curNode.leftChild != null)
                    {
                        smallerNumber += curNode.leftChild.childrenNum + curNode.leftChild.weight;
                    }
                }
            }
            long largerNumber = GetTreeNodeNumber() - smallerNumber - equalNumber;

            return new List<long>(3) { smallerNumber, equalNumber, largerNumber };
        }

        //O(LogN)
        public bool GetBiggestNodeNoMoreThanTarget(T max, out T result)
        {
            result = default(T);
            if (root == null)
            {
                return false;
            }

            var currentNode = root;
            bool found = false;
            while (currentNode != null)
            {
                if (CompareNode(currentNode.data, max) > 0)
                {
                    currentNode = currentNode.leftChild;
                }
                else if (CompareNode(currentNode.data, max) < 0)
                {
                    result = currentNode.data;
                    found = true;
                    currentNode = currentNode.rightChild;
                }
                else
                {
                    result = currentNode.data;
                    return true;
                }
            }

            return found;
        }

        //O(1)
        public long GetTreeNodeNumber()
        {
            if (root == null)
            {
                return 0;
            }

            return root.childrenNum + root.weight;
        }

        //O(N)
        public List<T> InOrderTraverse()
        {
            return InOrderTraverse(root);
        }

        //O(N)
        private List<T> InOrderTraverse(_2DAVLTreeNode<T> node)
        {
            var result = new List<T>();
            if (node.leftChild != null)
            {
                result.AddRange(InOrderTraverse(node.leftChild));
            }
            result.Add(node.data);
            if (node.rightChild != null)
            {
                result.AddRange(InOrderTraverse(node.rightChild));
            }

            return result;
        }

        //O(N)
        public List<T> InOrderTraverseFullInfo()
        {
            return InOrderTraverseFullInfo(root);
        }

        //O(N)
        private List<T> InOrderTraverseFullInfo(_2DAVLTreeNode<T> node)
        {
            var result = new List<T>();
            if (node.leftChild != null)
            {
                result.AddRange(InOrderTraverseFullInfo(node.leftChild));
            }
            result.Add(node.data);
            Console.WriteLine("Node: {0}, Weight: {7}, Parent: {1}, LeftChild: {2}, RightChild: {3}, LeftHeight: {4}, RightHeight: {5}, ChildrenNum: {6}",
                node.data.ToString(),
                node.parent == null ? "null" : node.parent.data.ToString(),
                node.leftChild == null ? "null" : node.leftChild.data.ToString(),
                node.rightChild == null ? "null" : node.rightChild.data.ToString(),
                node.leftHeight,
                node.rightHeight,
                node.childrenNum,
                node.weight);
            if (node.rightChild != null)
            {
                result.AddRange(InOrderTraverseFullInfo(node.rightChild));
            }

            return result;
        }

        private _2DAVLTreeNode<T> root = null;
        private Comparison<T> primaryComparison;
        private Comparison<T> secondaryComparison;
    }
}
