using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary
{
    public class AssemblyBrowser
    {
        private class ExtensionMethodInfo
        {
            public MemberInfo ParentClass
            { get; set; }

            public Node Node
            { get; set; }

            public Node ParentNode
            { get; set; }
        }

        private Assembly asm;
        private Dictionary<string, List<Node>> dictNameNodes;
        private List<ExtensionMethodInfo> listExtensionMethods;
        private Dictionary<MemberInfo, Node> dictClasses;

        public AssemblyBrowser()
        {
            dictNameNodes = new Dictionary<string, List<Node>>();
            listExtensionMethods = new List<ExtensionMethodInfo>();
            dictClasses = new Dictionary<MemberInfo, Node>();
        }

        public void LoadAssemblyFromFile(string asmFilePath)
        {
            try
            {
                asm = Assembly.LoadFrom(asmFilePath);
            }
            catch (Exception)
            {
                throw new LoadException();
            }
        }

        public Node GetTree()
        {
            if (asm == null)
            {
                throw new LoadException();
            }
            Type[] assemblyTypes = GetAsmTypes(asm);
            FillNameToNodeDict(assemblyTypes);
            ResolveExtensionMethods();
            return TreeConstructionHelper.ConstructTree(dictNameNodes);
        }

        private Type[] GetAsmTypes(Assembly asm)
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            catch (Exception)
            {
                asm = null;
                throw new LoadException();
            }

            types = types.Where(type => type != null && !type.IsNested
                && type.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
            if (types.Length == 0)
            {
                throw new LoadException();
            }
            return types;
        }

        private void FillNameToNodeDict(Type[] asmTypes)
        {
            dictNameNodes.Clear();
            listExtensionMethods.Clear();
            dictClasses.Clear();

            Node rootNode = new Node(NodeType.Namespace);
            FillNestedMembers(asmTypes, rootNode);
            for (int i = 0; i < rootNode.GetNodes().Count; i++)
            {
                if (!dictNameNodes.ContainsKey(asmTypes[i].Namespace))
                {
                    dictNameNodes.Add(asmTypes[i].Namespace, new List<Node>());
                }
                dictNameNodes[asmTypes[i].Namespace].Add(rootNode.GetNodes()[i]);
            }
        }

        private void FillNestedMembers(MemberInfo[] childMembers, Node parentNode)
        {
            foreach (MemberInfo member in childMembers)
            {
                Node childNode = TryMemberInfoToAssemblyNode(member);
                if (childNode != null)
                {
                    parentNode.AddNode(childNode);
                    if (childNode.NodeType == NodeType.Class)
                        dictClasses.Add(member, childNode);
                    else if (ExtensionMethodHelper.IsExtensionMethod(member))
                    {
                        ExtensionMethodHelper.ChangeNodeTypeToExtensionMethod(childNode);
                        listExtensionMethods.Add(new ExtensionMethodInfo()
                        {
                            ParentClass = ExtensionMethodHelper.GetExtensionMethodClass(member),
                            Node = childNode,
                            ParentNode = parentNode
                        });
                    }
                }
            }
        }

        private Node TryMemberInfoToAssemblyNode(MemberInfo memberInfo)
        {
            NodeType nodeType = NodeTypeDispatcher.GetNodeTypeByMemberInfo(memberInfo);
            if (nodeType == (NodeType)(-1))
                return null;
            Node node = new Node(nodeType);
            // dispatch textRepresentation
            node.TextRepresentation = TextRepresentationDispatcher.GetTextRepresentationFor(memberInfo, node.NodeType);

            if (CanHaveNestedMembers(node))
            {
                FillNestedMembers(GetMemberInfos(memberInfo as Type), node);
            }
            return node;
        }


        // only Class, Interface, Struct, Enum and Delegate can be nested
        private static bool CanHaveNestedMembers(Node node)
        {
            return node.NodeType >= NodeType.Class && node.NodeType <= NodeType.Enum;
        }

        // replace extension methods and correct their parents
        private void ResolveExtensionMethods()
        {
            foreach (ExtensionMethodInfo extMethodInfo in listExtensionMethods)
            {
                if (dictClasses.ContainsKey(extMethodInfo.ParentClass))
                {
                    extMethodInfo.ParentNode.GetNodes().Remove(extMethodInfo.Node);
                    dictClasses[extMethodInfo.ParentClass].GetNodes().Add(extMethodInfo.Node);
                }
            }
        }

        private MemberInfo[] GetMemberInfos(Type type)
        {
            return type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic).
                Where(member => ((member is MethodBase) ? !(member as MethodBase).IsSpecialName : true) &&
                ((member is FieldInfo) ? !(member as FieldInfo).IsSpecialName : true) &&
                ((member is PropertyInfo) ? !(member as PropertyInfo).IsSpecialName : true) &&
                member.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
        }
    }
}
