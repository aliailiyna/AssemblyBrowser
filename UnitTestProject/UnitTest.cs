using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssemblyBrowserLibrary;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest
    {
        private const string PATH = "../../../../TestLibrary/bin/Debug/netstandard2.0/TestLibrary.dll";

        [TestMethod]
        public void TestMethodLoad()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);
        }

        [TestMethod]
        public void TestMethodErrorPathLoad()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            Assert.ThrowsException<LoadException>(() => assemblyBrowser.LoadAssemblyFromFile("Error path"));
        }

        [TestMethod]
        public void TestMethodNotLoad()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            Assert.ThrowsException<LoadException>(() => assemblyBrowser.GetTree());
        }

        [TestMethod]
        public void TestMethodFolders()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);

            Node rootNode = assemblyBrowser.GetTree();

            Assert.IsNotNull(rootNode.GetNodes());
            Assert.AreEqual(2, rootNode.GetNodes().Count);

            Node testAssemblyNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("Пространство имён TestLibrary"));

            IEnumerable<Node> folderNodes = testAssemblyNode.GetNodes().Where(node => node.NodeType == NodeType.Folder);
            Assert.AreEqual(3, folderNodes.Count());
        }

        [TestMethod]
        public void TestMethodNamespaces()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);

            Node rootNode = assemblyBrowser.GetTree();

            Assert.IsNotNull(rootNode.GetNodes());
            Assert.AreEqual(2, rootNode.GetNodes().Count);
            IEnumerable<string> namespaces = rootNode.GetNodes().Select(node => node.TextRepresentation);
            Assert.AreEqual(2, namespaces.Count());
            Assert.IsTrue(namespaces.Contains("Пространство имён OtherNamespace"));
            Assert.IsTrue(namespaces.Contains("Пространство имён TestLibrary"));
        }

        [TestMethod]
        public void TestMethodClasses()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);

            Node rootNode = assemblyBrowser.GetTree();

            IEnumerable<string> namespaces = rootNode.GetNodes().Select(node => node.TextRepresentation);
            Node testAssemblyNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("Пространство имён TestLibrary"));

            IEnumerable<NodeType> namespaceTypes = testAssemblyNode.GetNodes().Select(node => node.NodeType);

            Assert.IsTrue(namespaceTypes.Contains(NodeType.Class));
            Assert.IsTrue(namespaceTypes.Contains(NodeType.Interface));
            Assert.IsTrue(namespaceTypes.Contains(NodeType.Struct));
            Assert.IsTrue(namespaceTypes.Contains(NodeType.Delegate));
            Assert.IsTrue(namespaceTypes.Contains(NodeType.Enum));

            Node testClassNode = testAssemblyNode.GetNodes().Find(node => node.TextRepresentation.Equals("Класс Class1"));

            IEnumerable<NodeType> testClassTypes = testClassNode.GetNodes().Select(node => node.NodeType);

            Assert.IsTrue(testClassTypes.Contains(NodeType.Enum));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Delegate));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Event));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Field));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Class));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Property));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Method));
        }

        [TestMethod]
        public void TestMethodClass1()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);

            Node rootNode = assemblyBrowser.GetTree();

            IEnumerable<string> namespaces = rootNode.GetNodes().Select(node => node.TextRepresentation);
            Node testAssemblyNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("Пространство имён TestLibrary"));

            IEnumerable<NodeType> namespaceTypes = testAssemblyNode.GetNodes().Select(node => node.NodeType);

            Node testClassNode = testAssemblyNode.GetNodes().Find(node => node.TextRepresentation.Equals("Класс Class1"));

            IEnumerable<NodeType> testClassTypes = testClassNode.GetNodes().Select(node => node.NodeType);

            Assert.IsTrue(testClassTypes.Contains(NodeType.Enum));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Delegate));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Event));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Field));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Class));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Property));
            Assert.IsFalse(testClassTypes.Contains(NodeType.Method));
        }

        [TestMethod]
        public void TestMethodExtensionMethods()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(PATH);

            Node rootNode = assemblyBrowser.GetTree();

            Node testAssemblyNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("Пространство имён TestLibrary"));
            IEnumerable<NodeType> namespaceTypes = testAssemblyNode.GetNodes().Select(node => node.NodeType);

            Node testClassExt = testAssemblyNode.GetNodes().Find(node => node.TextRepresentation.Equals("Класс ClassExt"));
            IEnumerable<Node> listTestClassExt = testClassExt.GetNodes().Where(node => node.NodeType == NodeType.ExtensionMethod);
            Assert.AreEqual(1, listTestClassExt.Count());

            Node testExtMethods = testAssemblyNode.GetNodes().Find(node => node.TextRepresentation.Equals("Класс ExtMethods"));
            IEnumerable<Node> listTestExtMethods = testExtMethods.GetNodes().Where(node => node.NodeType == NodeType.ExtensionMethod);
            Assert.AreEqual(1, listTestExtMethods.Count());
        }
    }
}
