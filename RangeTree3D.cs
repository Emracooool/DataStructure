using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class RangeTree3D<T>
    {
        public RangeTree3D(List<T> data, Comparison<T> mainComparison, Comparison<T> subComparison, Comparison<T> thirdComparison)
        {
            this.mainComparison = mainComparison;
            this.subComparison = subComparison;
            this.thirdComparison = thirdComparison;
            ConstructRangeTree3D(data);
        }

        private void ConstructRangeTree3D(List<T> data)
        {
            if (data.Count == 0)
            {
                return;
            }

            //Sort by mainComparison
            data.Sort(mainComparison);
            var sortedData = data.ToArray();

            //Construct leaf nodes
            var nodes = new RangeTreeNode3D<T>[sortedData.Length];
            var firstNode = new RangeTreeNode3D<T>()
            {
                data = sortedData[0],
                max = sortedData[0],
                leafNumber = 1,
                leafData = new List<T>() { sortedData[0] }
            };
            nodes[0] = firstNode;


            var lastData = sortedData[0];
            var lastNode = firstNode;
            var index = 1;

            for (int i = 1; i < sortedData.Length; i++)
            {
                var t = sortedData[i];
                if (CompareNode(lastData, sortedData[i]) == 0)
                {
                    lastNode.weight++;
                    lastNode.leafNumber++;
                    lastNode.leafData.Add(sortedData[i]);
                    continue;
                }

                var node = new RangeTreeNode3D<T>()
                {
                    data = sortedData[i],
                    max = sortedData[i],
                    leafNumber = 1,
                    leafData = new List<T>() { sortedData[i] }
                };
                nodes[index] = node;
                lastData = node.data;
                lastNode = node;
                index++;
            }

            //Construct subtree for each leaf node
            for (int i = 0; i < index; i++)
            {
                nodes[i].subTree = new RangeTree2D<T>(nodes[i].leafData, subComparison, thirdComparison);
            }

            //Traverse level up to root 
            ConstructBranchNodes(nodes, index);
        }

        private void ConstructBranchNodes(RangeTreeNode3D<T>[] nodes, int length)
        {
            if (length == 1)
            {
                root = nodes[0];
                root.leafData = null;
                return;
            }

            var newLength = length / 2;
            if (length % 2 == 1)
            {
                newLength++;
            }
            var newNodes = new RangeTreeNode3D<T>[newLength];

            for (int i = 0; i < length / 2; i++)
            {
                var leafData = nodes[i * 2].leafData;
                leafData.AddRange(nodes[i * 2 + 1].leafData);
                nodes[i * 2].leafData = null;
                nodes[i * 2 + 1].leafData = null;
                var node = new RangeTreeNode3D<T>()
                {
                    data = nodes[i * 2].max,
                    max = nodes[i * 2 + 1].max,
                    leafNumber = nodes[i * 2].leafNumber + nodes[i * 2 + 1].leafNumber,
                    leftChild = nodes[i * 2],
                    rightChild = nodes[i * 2 + 1],
                    leafData = leafData,
                    subTree = new RangeTree2D<T>(leafData, subComparison, thirdComparison)
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

        //O(LgNLgNLgN)
        public int GetRangeSearchCountOpen(T minX, T maxX, T minY, T maxY, T minZ, T maxZ)
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
                var IsMinLeft = CompareNode(minX, currentNode.data) <= 0;
                var isMaxLeft = CompareNode(maxX, currentNode.data) <= 0;

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
                if (CompareNode(minX, currentNode.data) <= 0)
                {
                    if (currentNode.rightChild != null)
                    {
                        //result += currentNode.rightChild.leafNumber;
                        result += currentNode.rightChild.subTree.GetRangeSearchCountOpen(minY, maxY, minZ, maxZ);
                    }
                    else if (CompareNode(minX, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        //result += currentNode.leafNumber;
                        result += currentNode.subTree.GetRangeSearchCountOpen(minY, maxY, minZ, maxZ);
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
                if (CompareNode(maxX, currentNode.data) <= 0)
                {
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    if (currentNode.leftChild != null)
                    {
                        //result += currentNode.leftChild.leafNumber;
                        result += currentNode.leftChild.subTree.GetRangeSearchCountOpen(minY, maxY, minZ, maxZ);
                    }
                    else
                    {
                        //Leaf Node
                        //result += currentNode.leafNumber;
                        result += currentNode.subTree.GetRangeSearchCountOpen(minY, maxY, minZ, maxZ);
                    }
                    currentNode = currentNode.rightChild;
                }
            }

            //Check Split Node if it is a leaf node
            if (splitNode.leftChild == null)
            {
                if (CompareNode(minX, splitNode.data) < 0 && CompareNode(maxX, splitNode.data) > 0)
                {
                    //result += currentNode.leafNumber;
                    result += splitNode.subTree.GetRangeSearchCountOpen(minY, maxY, minZ, maxZ);
                }
            }
            return result;
        }


        private int CompareNode(T a, T b)
        {
            return mainComparison.Invoke(a, b);
        }


        private Comparison<T> mainComparison;
        private Comparison<T> subComparison;
        private Comparison<T> thirdComparison;
        private RangeTreeNode3D<T> root = null;

    }
}
