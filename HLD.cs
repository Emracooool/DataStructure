using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    //Heavy Light Decomposition
    public class HLD
    {
        public HLD(List<int[]> edges)
        {
            root = new HLDTreeNode()
            {
                id = 0,
                parent = null,
                children = new List<HLDTreeNode>(),
                value = int.MinValue
            };
            segNodes.Add(new List<HLDTreeNode>() { root });
            nodes = new HLDTreeNode[edges.Count + 1];
            nodes[0] = root;

            BuildTree(edges);
            HeavyLightDecomposition(root, 0);
            CreateSegTrees();
        }

        private void BuildTree(List<int[]> edges)
        {
            var N = edges.Count;
            var sortedEdges = new List<int[]>[N + 1];

            for (int i = 0; i < N; i++)
            {
                if (sortedEdges[edges[i][0] - 1] == null)
                {
                    sortedEdges[edges[i][0] - 1] = new List<int[]>();
                }
                sortedEdges[edges[i][0] - 1].Add(new int[2] { edges[i][1], edges[i][2] });
            }

            var nodeIdStack = new Stack<int>();
            nodeIdStack.Push(0);
            var parentStack = new Stack<HLDTreeNode>();
            parentStack.Push(root);

            while (nodeIdStack.Count > 0)
            {
                var id = nodeIdStack.Pop();
                var parent = parentStack.Pop();
                if (sortedEdges[id] == null)
                {
                    //leaf
                    continue;
                }

                foreach (var edge in sortedEdges[id])
                {
                    var node = new HLDTreeNode
                    {
                        id = edge[0] - 1,
                        parent = parent,
                        children = new List<HLDTreeNode>(),
                        value = edge[1],
                        depth = parent.depth + 1
                    };


                    nodes[edge[0] - 1] = node;
                    parent.children.Add(node);
                    parent.size++;
                    nodeIdStack.Push(edge[0] - 1);
                    parentStack.Push(node);
                }
            }

            //Currently all size is the number of its direct children, update
            UpdateSize(root);
        }

        private void UpdateSize(HLDTreeNode node)
        {
            if (node.children.Count > 0)
            {
                foreach (var child in node.children)
                {
                    UpdateSize(child);
                }

                var t = 1;
                foreach (var child in node.children)
                {
                    t += child.size;
                }
                node.size = t;
            }
            else
            {
                node.size = 1;
            }
        }

        private void HeavyLightDecomposition(HLDTreeNode node, int index)
        {
            if (node.children.Count == 0)
            {
                return;
            }

            HLDTreeNode maxchild = null;
            var max = -1;
            foreach (var child in node.children)
            {
                if (child.size > max)
                {
                    maxchild = child;
                    max = child.size;
                }
            }

            maxchild.segId = index;
            maxchild.segPos = segNodes[index].Count;
            segNodes[index].Add(maxchild);
            HeavyLightDecomposition(maxchild, index);

            foreach (var child in node.children)
            {
                if (child != maxchild)
                {
                    var newIndex = segNodes.Count;
                    child.segId = newIndex;
                    child.segPos = 0;
                    segNodes.Add(new List<HLDTreeNode>()
                    {
                        child
                    });
                    HeavyLightDecomposition(child, newIndex);
                }
            }
        }

        private void CreateSegTrees()
        {
            var allVal = new List<int>();
            segTrees = new List<SegTreeMax>(segNodes.Count);

            for (int i = 0; i < segNodes.Count; i++)
            {
                var values = new int[segNodes[i].Count];
                for (int j = 0; j < segNodes[i].Count; j++)
                {
                    values[j] = segNodes[i][j].value;
                    allVal.Add(segNodes[i][j].value);
                    segNodes[i][j].fullSegPos = allVal.Count - 1;
                }
                var tree = new SegTreeMax(values, int.MinValue);
                segTrees.Add(tree);
            }

            //Create a segment tree containing all nodes
            fullSegTree = new SegTreeMax(allVal.ToArray(), 0);
        }

        public int GetMaxEdge(int i, int j)
        {
            var lca = FindLCA(nodes[i], nodes[j]);

            return Math.Max(GetMaxEdge(nodes[i], lca), GetMaxEdge(nodes[j], lca));
        }

        private int GetMaxEdge(HLDTreeNode start, HLDTreeNode end)
        {
            if (start.segId != end.segId)
            {
                var max = segTrees[start.segId].GetCombine(0, start.segPos);
                return Math.Max(max, GetMaxEdge(segNodes[start.segId][0].parent, end));
            }
            else
            {
                if(end.segPos == start.segPos)
                {
                    return int.MinValue;
                }
                else
                {
                    return segTrees[start.segId].GetCombine(start.segPos, end.segPos + 1);
                }
            }
        }

        public void UpdateValue(int i, int newValue)
        {
            var node = nodes[i];

            while (node != null)
            {
                segTrees[node.segId].UpdateValue(node.segPos, newValue);
                node = segNodes[node.segId][0].parent;
            }
        }

        private HLDTreeNode FindLCA(HLDTreeNode p, HLDTreeNode q)
        {
            if (p.depth < q.depth)
            {
                var t = p;
                p = q;
                q = t;
            }

            if (p.segId == q.segId)
            {
                //on same segment tree
                return q;
            }

            var pa = segNodes[p.segId][0];
            if (pa.depth >= q.depth)
            {
                return FindLCA(pa.parent, q);
            }

            var qa = segNodes[q.segId][0];
            if (qa.depth <= pa.depth)
            {
                return FindLCA(pa.parent, qa.parent);
            }

            return FindLCA(p, qa.parent);
        }

        private HLDTreeNode root;
        private HLDTreeNode[] nodes;
        private List<List<HLDTreeNode>> segNodes = new List<List<HLDTreeNode>>();
        private List<SegTreeMax> segTrees;
        private SegTreeMax fullSegTree;
    }

    public class HLDTreeNode
    {
        public int id;
        public HLDTreeNode parent;
        public List<HLDTreeNode> children;
        public int value;//the edge value from this node to its parent

        //For HLD
        public int size;//number of child nodes
        public int segId;//the seg tree id
        public int segPos;//position in seg tree
        public int fullSegPos;//id in full seg tree
        public int depth;
    }
}
