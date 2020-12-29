using System.Collections.Generic;

namespace AssemblyBrowserLibrary
{
    public class Node
    {
        private List<Node> nodes;
        public NodeType NodeType { get; internal set; }
        public string TextRepresentation { get; internal set; }

        public Node(NodeType nodeType)
        {
            NodeType = nodeType;
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        internal Node AddNode(Node assemblyNode)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            nodes.Add(assemblyNode);
            return assemblyNode;
        }

        internal void AddAll(List<Node> assemblyNodes)
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            nodes.AddRange(assemblyNodes);
        }
    }
}
