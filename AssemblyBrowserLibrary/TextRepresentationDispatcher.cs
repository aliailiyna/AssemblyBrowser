using System;
using System.Linq;
using System.Reflection;

namespace AssemblyBrowserLibrary
{
    internal class TextRepresentationDispatcher
    {
        public static string GetTextRepresentationFor(MemberInfo memberInfo, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Class: 
                    return ClassToText(memberInfo);
                case NodeType.Interface:
                    return InterfaceToText(memberInfo);
                case NodeType.Struct:
                    return StructToText(memberInfo);
                case NodeType.Enum:
                    return EnumToText(memberInfo);
                case NodeType.Property:
                    return PropertyToText(memberInfo);
                case NodeType.Field:
                    return FieldToText(memberInfo);
                case NodeType.Method:
                    return MethodToText(memberInfo);
                case NodeType.Delegate:
                    return DelegateToText(memberInfo);
                case NodeType.Event:
                    return EventToText(memberInfo);
                default: 
                    return null;
            }
        }

        private static string ClassToText(MemberInfo memberInfo)
        {
            return string.Format("Класс {0}", memberInfo.Name);
        }

        private static string InterfaceToText(MemberInfo memberInfo)
        {
            return string.Format("Интерфейс {0}", memberInfo.Name);
        }

        private static string StructToText(MemberInfo memberInfo)
        {
            return string.Format("Структура {0}", memberInfo.Name);
        }

        private static string EnumToText(MemberInfo memberInfo)
        {
            return string.Format("Перечисление {0}", memberInfo.Name);
        }

        private static string PropertyToText(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            return string.Format("Свойство {0}: {1}", propertyInfo.Name, propertyInfo.PropertyType.Name);
        }

        private static string FieldToText(MemberInfo memberInfo)
        {
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            return string.Format("Поле {0}: {1}", fieldInfo.Name, fieldInfo.FieldType.Name);
        }

        private static string MethodToText(MemberInfo memberInfo)
        {
            MethodBase methodBase = memberInfo as MethodBase;
            string str = "Метод";
            if (ExtensionMethodHelper.IsExtensionMethod(memberInfo))
            {
                str += " расширения";
            }
            string methodText = string.Format("{0} {1}({2})", str, methodBase.Name,
                string.Join(", ", methodBase.GetParameters().Select(o => o.ParameterType.Name).ToArray()));
            return methodBase is MethodInfo ? methodText + $": {(methodBase as MethodInfo).ReturnType.Name}" : methodText;
        }

        private static string DelegateToText(MemberInfo memberInfo)
        {
            Type delegateType = memberInfo as Type;
            MethodBase invokeMethod = delegateType.GetMethod("Invoke");
            return string.Format("Делегат {0}({1}): {2}", delegateType.Name,
                string.Join(", ", invokeMethod.GetParameters().Select(o => o.ParameterType.Name).ToArray()),
                (invokeMethod as MethodInfo).ReturnType.Name);
        }

        private static string EventToText(MemberInfo memberInfo)
        {
            EventInfo eventInfo = (EventInfo)memberInfo;
            MethodInfo invokeMethodInfo = eventInfo.EventHandlerType.GetMethod("Invoke");
            return string.Format("Событие {0}: {1}({2})", eventInfo.Name, eventInfo.EventHandlerType.Name,
                string.Join(", ", invokeMethodInfo.GetParameters().Select(o => o.ParameterType.Name).ToArray()));
        }
    }
}
