using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructure
{
    public class Trie
    {
        public void BuildTrie(List<string> strs)
        {
            root.total = strs.Count;
            BuildTrie(strs, root);
        }

        private void BuildTrie(List<string> strs, TrieNode root)
        {
            var dic = new Dictionary<char, List<string>>();

            foreach (var s in strs)
            {
                if (s.Length == root.level)
                {
                    root.count++;
                    continue;
                }

                if (dic.ContainsKey(s[root.level]))
                {
                    dic[s[root.level]].Add(s);
                }
                else
                {
                    dic.Add(s[root.level], new List<string>() { s });
                }
            }

            foreach (var pair in dic)
            {
                var node = new TrieNode
                {
                    data = pair.Value[0].Substring(0, root.level + 1),
                    level = root.level + 1,
                    total = pair.Value.Count
                };

                root.children.Add(node);
                BuildTrie(pair.Value, node);
            }

        }

        public int GetNodeCount()
        {
            return GetNodeCount(root);
        }

        private int GetNodeCount(TrieNode root)
        {
            var result = 1;

            foreach (var child in root.children)
            {
                result += GetNodeCount(child);
            }

            return result;
        }

        public TrieNode root = new TrieNode
        {
            data = "",
            level = 0
        };
    }

    public class TrieNode
    {
        public string data;
        public int level;
        public int count = 0;
        public int total = 0;
        public List<TrieNode> children = new List<TrieNode>();
    }
}
