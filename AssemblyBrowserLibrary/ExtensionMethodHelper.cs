using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLibrary
{
    internal class ExtensionMethodHelper
    {
        // by attribute
        internal static bool IsExtensionMethod(MemberInfo member)
        {
            return member is MethodBase && (member as MethodBase).GetCustomAttribute<ExtensionAttribute>() != null;
        }

        internal static void ChangeNodeTypeToExtensionMethod(Node node)
        {
            node.NodeType = NodeType.ExtensionMethod;
        }

        // by this parameter
        internal static MemberInfo GetExtensionMethodClass(MemberInfo member)
        {
            return (member as MethodBase).GetParameters()[0].ParameterType;
        }
    }
}
