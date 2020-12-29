using System.Collections.Generic;

namespace AssemblyBrowserLibrary
{
    internal class TreeConstructionHelper
    {
        private static readonly NodeComparer nodeComparer;

        static TreeConstructionHelper()
        {
            nodeComparer = new NodeComparer();
        }

        public static Node ConstructTree(Dictionary<string, List<Node>> namespaceToTypesMap)
        {
            Node rootNode = new Node(NodeType.Namespace);
            foreach (KeyValuePair<string, List<Node>> namespaceToTypes in namespaceToTypesMap)
            {
                AddFoldersStructure(rootNode, namespaceToTypes.Key, namespaceToTypes.Value);
            }
            SortNodes(rootNode);
            return rootNode;
        }

        private static void AddFoldersStructure(Node rootNode, string namespaceName, List<Node> nodes)
        {
            string[] namespaceParts = namespaceName.Split('.');
            Node currNode = CreateOrGetNamespaceNode(rootNode, namespaceParts[0]);
            for (int i = 1; i < namespaceParts.Length; i++)
            {
                currNode = CreateOrGetFolderNode(currNode, namespaceParts[i]);
            }
            currNode.AddAll(nodes);
        }

        // Namespace textRepresentation
        private static Node CreateOrGetNamespaceNode(Node rootNode, string rootNamespaceName)
        {
            return CreateOrGetNode(rootNode, NodeType.Namespace, "Пространство имён " + rootNamespaceName);
        }

        // Folder textRepresentation
        private static Node CreateOrGetFolderNode(Node rootNode, string rootNamespaceName)
        {
            return CreateOrGetNode(rootNode, NodeType.Folder, "Папка " + rootNamespaceName);
        }

        private static Node CreateOrGetNode(Node rootNode, NodeType nodeType, string textRepresentation)
        {
            if (rootNode.GetNodes() == null)
            {
                return rootNode.AddNode(CreateEmptyNode(nodeType, textRepresentation));
            }

            Node foundNode = rootNode.GetNodes().Find(node => node.NodeType == nodeType && node.TextRepresentation.Equals(textRepresentation));
            if (foundNode == null)
                foundNode = rootNode.AddNode(CreateEmptyNode(nodeType, textRepresentation));
            return foundNode;
        }

        private static Node CreateEmptyNode(NodeType nodeType, string textRepresentation)
        {
            Node node = new Node(nodeType);
            node.TextRepresentation = textRepresentation;
            return node;
        }

        private static void SortNodes(Node currNode)
        {
            if (currNode.GetNodes() == null || !IsStorageNode(currNode))
                return;
            currNode.GetNodes().Sort(nodeComparer);
            foreach (Node node in currNode.GetNodes())
                SortNodes(node);
        }

        // storage nodes are only Namespace and Folder
        internal static bool IsStorageNode(Node node)
        {
            return node.NodeType == NodeType.Namespace || node.NodeType == NodeType.Folder;
        }
    }

    class NodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (TreeConstructionHelper.IsStorageNode(x) && !TreeConstructionHelper.IsStorageNode(y))
                return -1;
            if (!TreeConstructionHelper.IsStorageNode(x) && TreeConstructionHelper.IsStorageNode(y))
                return 1;
            return x.TextRepresentation.CompareTo(y.TextRepresentation);
        }

    }
}
