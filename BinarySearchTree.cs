using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    class BinarySearchTree<T>
    {
        public BinarySearchTree(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        public bool InsertNode(T node)
        {
            var newNode = new TreeNode<T>()
            {
                data = node
            };

            if (root == null)
            {
                root = newNode;
                return true;
            }

            var currentNode = root;
            while (true)
            {
                var compareResult = CompareNode(node, currentNode.data);
                currentNode.childrenNum++;
                if (compareResult >= 0)
                {
                    if (currentNode.rightChild == null)
                    {
                        currentNode.rightChild = newNode;
                        newNode.parent = currentNode;
                        break;
                    }
                    else
                    {
                        currentNode = currentNode.rightChild;
                    }
                }
                else if (compareResult < 0)
                {
                    if (currentNode.leftChild == null)
                    {
                        currentNode.leftChild = newNode;
                        newNode.parent = currentNode;
                        break;
                    }
                    else
                    {
                        currentNode = currentNode.leftChild;
                    }
                }
            }

            return true;
        }

        public bool RemoveNode(T node)
        {
            if (root == null)
            {
                return false;
            }

            var currentNode = root;
            var isLeft = true;
            while (true)
            {
                var compareResult = CompareNode(node, currentNode.data);
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
                    if (currentNode.leftChild == null)
                    {
                        if(currentNode.parent == null)
                        {
                            //root node
                            root = currentNode.rightChild;
                        }
                        else
                        {
                            var parent = currentNode.parent;
                            if(isLeft)
                            {
                                parent.leftChild = currentNode.rightChild;
                            }
                            else
                            {
                                parent.rightChild = currentNode.rightChild;
                            }
                        }
                        MinusParentChildrenNum(currentNode);
                        currentNode = null;//Memory Leak??
                    }
                    else if (currentNode.rightChild == null)
                    {
                        if (currentNode.parent == null)
                        {
                            //root node
                            root = currentNode.leftChild;
                        }
                        else
                        {
                            var parent = currentNode.parent;
                            if (isLeft)
                            {
                                parent.leftChild = currentNode.leftChild;
                            }
                            else
                            {
                                parent.rightChild = currentNode.leftChild;
                            }
                        }
                        MinusParentChildrenNum(currentNode);
                        currentNode = null;
                    }
                    else
                    {
                        //Get the next node in InOrderTraverse
                        var nextNode = GetNextInOrderNode(currentNode);
                        if(nextNode.parent == currentNode)
                        {
                            nextNode.leftChild = currentNode.leftChild;
                            MinusParentChildrenNum(currentNode);
                            currentNode = null;
                        }
                        else
                        {
                            //Copy the value of nextNode to currentNode, then delete nextNode
                            currentNode.data = nextNode.data;
                            nextNode.parent.leftChild = nextNode.rightChild;
                            if(nextNode.rightChild != null)
                            {
                                nextNode.rightChild.parent = nextNode.parent;
                            }
                            MinusParentChildrenNum(nextNode);
                            nextNode = null;
                        }
                    }
                    return true;
                }
            }
        }

        public List<T> InOrderTraverse()
        {
            return InOrderTraverse(root);
        }

        private List<T> InOrderTraverse(TreeNode<T> node)
        {
            var result = new List<T>();
            if(node.leftChild != null)
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

        private void MinusParentChildrenNum(TreeNode<T> node)
        {
            var parent = node.parent;
            while (parent != null)
            {
                parent.childrenNum--;
                parent = parent.parent;
            }
        }

        private TreeNode<T> GetNextInOrderNode(TreeNode<T> node)
        {
            if (node.rightChild != null)
            {
                var currentNode = node.rightChild;
                while (true)
                {
                    if(currentNode.leftChild != null)
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

        public int GetNodeNumbers()
        {
            return GetChildrenNumbers(root) + 1;
        }

        private int GetChildrenNumbers(TreeNode<T> rootNode)
        {
            return rootNode.childrenNum;
        }

        public T FindNode(T node)
        {
            return node;
        }

        public int GetEqualNodeNumber(T node)
        {
            return GetEqualNodeNumber(node, root);
        }

        private int GetEqualNodeNumber(T node, TreeNode<T> rootNode)
        {
            if (rootNode == null)
            {
                return 0;
            }

            var currentNode = rootNode;
            while (true)
            {
                var compareResult = CompareNode(node, currentNode.data);
                if (compareResult > 0)
                {
                    if (currentNode.rightChild == null)
                    {
                        return 0;
                    }
                    else
                    {
                        currentNode = currentNode.rightChild;
                    }
                }
                else if (compareResult < 0)
                {
                    if (currentNode.leftChild == null)
                    {
                        return 0;
                    }
                    else
                    {

                        currentNode = currentNode.leftChild;
                    }
                }
                else
                {
                    var left = GetEqualNodeNumber(node, currentNode.leftChild);
                    var right = GetEqualNodeNumber(node, currentNode.rightChild);

                    return left + right + 1;
                }
            }
        }

        protected int CompareNode(T a, T b)
        {
            return comparison.Invoke(a, b);
        }

        protected TreeNode<T> root = null;
        protected Comparison<T> comparison;
    }
}
