using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class RangeTree2D<T>
    {
        public RangeTree2D(List<T> data, Comparison<T> mainComparison, Comparison<T> subComparison)
        {
            this.mainComparison = mainComparison;
            this.subComparison = subComparison;
            ConstructRangeTree2D(data);
        }

        private void ConstructRangeTree2D(List<T> data)
        {
            if (data.Count == 0)
            {
                return;
            }

            //Sort by mainComparison
            data.Sort(mainComparison);
            var sortedData = data.ToArray();

            //Construct leaf nodes
            var nodes = new RangeTreeNode2D<T>[sortedData.Length];
            var firstNode = new RangeTreeNode2D<T>()
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
                if (CompareNode(lastData, sortedData[i]) == 0)
                {
                    lastNode.weight++;
                    lastNode.leafNumber++;
                    lastNode.leafData.Add(sortedData[i]);
                    continue;
                }

                var node = new RangeTreeNode2D<T>()
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
                nodes[i].subTree = new RangeTree<T>(nodes[i].leafData, subComparison);
            }

            //Traverse level up to root 
            ConstructBranchNodes(nodes, index);
        }

        private void ConstructBranchNodes(RangeTreeNode2D<T>[] nodes, int length)
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
            var newNodes = new RangeTreeNode2D<T>[newLength];

            for (int i = 0; i < length / 2; i++)
            {
                var leafData = nodes[i * 2].leafData;
                leafData.AddRange(nodes[i * 2 + 1].leafData);
                nodes[i * 2].leafData = null;
                nodes[i * 2 + 1].leafData = null;
                var node = new RangeTreeNode2D<T>()
                {
                    data = nodes[i * 2].max,
                    max = nodes[i * 2 + 1].max,
                    leafNumber = nodes[i * 2].leafNumber + nodes[i * 2 + 1].leafNumber,
                    leftChild = nodes[i * 2],
                    rightChild = nodes[i * 2 + 1],
                    leafData = leafData,
                    subTree = new RangeTree<T>(leafData, subComparison)
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

        //O(lgNlgN), tested
        //Get the min Y in square(minX, maxX), [minY, maxY]
        public bool GetMinYInRangeOOCC(T minX, T maxX, T minY, T maxY, out T result)
        {
            result = maxY;

            if (root == null)
            {
                return false;
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
                return false;
            }

            //Get Min Y from Split to Min
            var found = false;
            currentNode = splitNode.leftChild;
            while (currentNode != null)
            {
                if (CompareNode(minX, currentNode.data) <= 0)
                {
                    if (currentNode.rightChild != null)
                    {
                        if (currentNode.rightChild.subTree.GetMinXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) < 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    else if (CompareNode(minX, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        if (currentNode.subTree.GetMinXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) < 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    currentNode = currentNode.rightChild;
                }
            }

            //Get Min Y from Split to Max
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
                        if (currentNode.leftChild.subTree.GetMinXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) < 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    else
                    {
                        //Leaf Node
                        if (currentNode.subTree.GetMinXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) < 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    currentNode = currentNode.rightChild;
                }
            }

            //Check Split Node if it is a leaf node
            if (splitNode.leftChild == null)
            {
                if (CompareNode(minX, splitNode.data) < 0 && CompareNode(maxX, splitNode.data) > 0)
                {
                    if (splitNode.subTree.GetMinXInRangeClose(minY, maxY, out T tmp))
                    {
                        found = true;
                        if (CompareNodeSub(tmp, result) < 0)
                        {
                            result = tmp;
                        }
                    }
                }
            }

            return found;
        }
        
        //O(lgNlgN), tested
        //Get the max Y in square(minX, maxX), [minY, maxY]
        public bool GetMaxYInRangeOOCC(T minX, T maxX, T minY, T maxY, out T result)
        {
            result = minY;

            if (root == null)
            {
                return false;
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

            if(currentNode == null)
            {
                return false;
            }

            //Get Min Y from Split to Min
            var found = false;
            currentNode = splitNode.leftChild;
            while (currentNode != null)
            {
                if (CompareNode(minX, currentNode.data) <= 0)
                {
                    if (currentNode.rightChild != null)
                    {
                        if (currentNode.rightChild.subTree.GetMaxXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) > 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    else if (CompareNode(minX, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        if (currentNode.subTree.GetMaxXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) > 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    currentNode = currentNode.leftChild;
                }
                else
                {
                    currentNode = currentNode.rightChild;
                }
            }

            //Get Max Y from Split to Max
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
                        if (currentNode.leftChild.subTree.GetMaxXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) > 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    else
                    {
                        //Leaf Node
                        if (currentNode.subTree.GetMaxXInRangeClose(minY, maxY, out T tmp))
                        {
                            found = true;
                            if (CompareNodeSub(tmp, result) > 0)
                            {
                                result = tmp;
                            }
                        }
                    }
                    currentNode = currentNode.rightChild;
                }
            }

            //Check Split Node if it is a leaf node
            if(splitNode.leftChild == null)
            {
                if (CompareNode(minX, splitNode.data) < 0 && CompareNode(maxX, splitNode.data) > 0)
                {
                    if (splitNode.subTree.GetMaxXInRangeClose(minY, maxY, out T tmp))
                    {
                        found = true;
                        if (CompareNodeSub(tmp, result) > 0)
                        {
                            result = tmp;
                        }
                    }
                }
            }

            return found;
        }

        //O(LgNLgN)
        public int GetRangeSearchCountOpen(T minX, T maxX, T minY, T maxY)
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
                        result += currentNode.rightChild.subTree.GetRangeSearchCountOpen(minY, maxY);
                    }
                    else if (CompareNode(minX, currentNode.data) < 0)
                    {
                        //Leaf Node and smaller
                        //result += currentNode.leafNumber;
                        result += currentNode.subTree.GetRangeSearchCountOpen(minY, maxY);
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
                        result += currentNode.leftChild.subTree.GetRangeSearchCountOpen(minY, maxY);
                    }
                    else
                    {
                        //Leaf Node
                        //result += currentNode.leafNumber;
                        result += currentNode.subTree.GetRangeSearchCountOpen(minY, maxY);
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
                    result += splitNode.subTree.GetRangeSearchCountOpen(minY, maxY);
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

        private List<T> GetAllLeafData(RangeTreeNode2D<T> node)
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
            return mainComparison.Invoke(a, b);
        }

        private int CompareNodeSub(T a, T b)
        {
            return subComparison.Invoke(a, b);
        }


        private Comparison<T> mainComparison;
        private Comparison<T> subComparison;
        private RangeTreeNode2D<T> root = null;
    }
}
