using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class RangeTree<T>
    {
        public RangeTree(T[] sortedData, Comparison<T> comparison)
        {
            this.comparison = comparison;
            ConstructRangeTree(sortedData);
        }

        public RangeTree(List<T> data, Comparison<T> comparison)
        {
            this.comparison = comparison;
            data.Sort(comparison);
            ConstructRangeTree(data.ToArray());
        }

        private void ConstructRangeTree(T[] sortedData)
        {
            if (sortedData.Length == 0)
            {
                return;
            }

            //Construct leaf nodes
            var nodes = new RangeTreeNode<T>[sortedData.Length];
            var firstNode = new RangeTreeNode<T>()
            {
                data = sortedData[0],
                max = sortedData[0],
                leafNumber = 1
            };
            nodes[0] = firstNode;


            var lastData = sortedData[0];
            var lastNode = firstNode;
            var index = 1;

            for (int i = 1; i < sortedData.Length; i++)
            {
                if (CompareNode(lastData, sortedData[i]) == 0)
                {
                    lastNode.weight++;
                    lastNode.leafNumber++;
                    continue;
                }

                var node = new RangeTreeNode<T>()
                {
                    data = sortedData[i],
                    max = sortedData[i],
                    leafNumber = 1
                };
                nodes[index] = node;
                lastData = node.data;
                lastNode = node;
                index++;
            }

            //Traverse level up to root 
            ConstructBranchNodes(nodes, index);
        }

        private void ConstructBranchNodes(RangeTreeNode<T>[] nodes, int length)
        {
            if (length == 1)
            {
                root = nodes[0];
                return;
            }

            var newLength = length / 2;
            if (length % 2 == 1)
            {
                newLength++;
            }
            var newNodes = new RangeTreeNode<T>[newLength];

            for (int i = 0; i < length / 2; i++)
            {
                var node = new RangeTreeNode<T>()
                {
                    data = nodes[i * 2].max,
                    max = nodes[i * 2 + 1].max,
                    leafNumber = nodes[i * 2].leafNumber + nodes[i * 2 + 1].leafNumber,
                    leftChild = nodes[i * 2],
                    rightChild = nodes[i * 2 + 1]
                };
                nodes[i * 2].parent = node;
                nodes[i * 2 + 1].parent = node;

                newNodes[i] = node;
            }

            if (length % 2 == 1)
            {
                newNodes[newLength - 1] = nodes[length - 1];
            }

            ConstructBranchNodes(newNodes, newLength);
        }


        //O(lgN)
        public bool GetMinXInRangeClose(T min, T max, out T result)
        {
            result = max;
            if (root == null)
            {
                return false;
            }

            if(CompareNode(max, min) < 0)
            {
                return false;
            }

            //Find SplitNode
            var currentNode = root;
            var splitNode = root;
            var found = false;
            while (currentNode != null)
            {
                var compare = CompareNode(min, currentNode.data);
                if (compare > 0)
                {
                    currentNode = currentNode.rightChild;
                }
                else if (compare == 0)
                {
                    result = currentNode.data;
                    return true;
                }
                else
                {
                    found = true;
                    result = currentNode.data;
                    currentNode = currentNode.leftChild;
                }
            }

            if(found && CompareNode(result, max) <= 0)
            {
                return true;
            }

            return false;
        }

        //O(lgN)
        public bool GetMaxXInRangeClose(T min, T max, out T result)
        {
            result = min;
            if (root == null)
            {
                return false;
            }

            if (CompareNode(max, min) < 0)
            {
                return false;
            }

            //Find SplitNode
            var currentNode = root;
            var found = false;
            while (currentNode != null)
            {
                var compare = CompareNode(max, currentNode.data);
                if (compare > 0)
                {
                    found = true;
                    result = currentNode.data;
                    currentNode = currentNode.rightChild;
                }
                else if (compare == 0)
                {
                    result = currentNode.data;
                    return true;
                }
                else
                {
                    currentNode = currentNode.leftChild;
                }
            }

            if (found && CompareNode(result, min) >= 0)
            {
                return true;
            }

            return false;
        }


        //O(LgN)
        public int GetRangeSearchCountOpen(T min, T max)
        {
            if (root == null)
            {
                return 0;
            }

            //Find SplitNode
            var currentNode = root;
            var splitNode = root;
            while (currentNode != null)
            {
                var IsMinLeft = CompareNode(min, currentNode.data) <= 0;
                var isMaxLeft = CompareNode(max, currentNode.data) <= 0;

                if (IsMinLeft == isMaxLeft)
                {
                    currentNode = IsMinLeft ? currentNode.leftChild : currentNode.rightChild;
                }
                else
                {
                    splitNode = currentNode;
                    break;
                }
            }

            if (currentNode == null)
            {
                return 0;
            }

            //Get Leaf Number from Split to Min
            var result = 0;
            currentNode = splitNode.leftChild;
            while (currentNode != null)
            {
                if (CompareNode(min, currentNode.data) <= 0)
                {
                    if (currentNode.rightChild != null)
                    {
                        result += currentNode.rightChild.leafNumber;
                    }
                    else if (CompareNode(min, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        result += currentNode.leafNumber;
                    }
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    currentNode = currentNode.rightChild;
                }
            }

            //Get Leaf Number from Split to Max
            currentNode = splitNode.rightChild;
            while (currentNode != null)
            {
                if (CompareNode(max, currentNode.data) <= 0)
                {
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    if (currentNode.leftChild != null)
                    {
                        result += currentNode.leftChild.leafNumber;
                    }
                    else
                    {
                        //Leaf Node
                        result += currentNode.leafNumber;
                    }
                    currentNode = currentNode.rightChild;
                }
            }

            //Check Split Node if it is a leaf node
            if (splitNode.leftChild == null)
            {
                if (CompareNode(min, splitNode.data) < 0 && CompareNode(max, splitNode.data) > 0)
                {
                    //result += currentNode.leafNumber;
                    result += splitNode.weight;
                }
            }

            return result;
        }

        //O(LgN+K)
        public List<T> GetRangeSearchListOpen(T min, T max)
        {
            var result = new List<T>();
            if (root == null)
            {
                return result;
            }

            //Find SplitNode
            var currentNode = root;
            var splitNode = root;
            while (currentNode != null)
            {
                var IsMinLeft = CompareNode(min, currentNode.data) <= 0;
                var isMaxLeft = CompareNode(max, currentNode.data) <= 0;

                if (IsMinLeft == isMaxLeft)
                {
                    currentNode = IsMinLeft ? currentNode.leftChild : currentNode.rightChild;
                }
                else
                {
                    splitNode = currentNode;
                    break;
                }
            }

            //Get Leaf Number from Split to Min
            currentNode = splitNode.leftChild;
            while (currentNode != null)
            {
                if (CompareNode(min, currentNode.data) <= 0)
                {
                    if (currentNode.rightChild != null)
                    {
                        result.AddRange(GetAllLeafData(currentNode.rightChild));
                    }
                    else if (CompareNode(min, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        result.AddRange(GetAllLeafData(currentNode));
                    }
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    currentNode = currentNode.rightChild;
                }
            }

            //Get Leaf Number from Split to Max
            currentNode = splitNode.rightChild;
            while (currentNode != null)
            {
                if (CompareNode(max, currentNode.data) <= 0)
                {
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    if (currentNode.leftChild != null)
                    {
                        result.AddRange(GetAllLeafData(currentNode.leftChild));
                    }
                    else
                    {
                        //Leaf Node
                        result.AddRange(GetAllLeafData(currentNode));
                    }
                    currentNode = currentNode.rightChild;
                }
            }

            return result;
        }

        private List<T> GetAllLeafData(RangeTreeNode<T> node)
        {
            var result = new List<T>();

            if (node.leftChild == null)
            {
                for (int i = 0; i < node.weight; i++)
                {
                    result.Add(node.data);
                }
            }

            result.AddRange(GetAllLeafData(node.leftChild));
            result.AddRange(GetAllLeafData(node.rightChild));

            return result;
        }

        private int CompareNode(T a, T b)
        {
            return comparison.Invoke(a, b);
        }


        private Comparison<T> comparison;
        private RangeTreeNode<T> root = null;
    }
}
