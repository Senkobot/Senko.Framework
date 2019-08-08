using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Senko.Common
{
    public static class TypeExtensions
    {
        public static Type ToUnderlyingType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        public static Type GetUnderlyingType(this object obj)
        {
            return ToUnderlyingType(obj.GetType());
        }

        /// <summary>
        ///     Get all the attribute from the the declaring type.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <returns>The attributes.</returns>
        public static IEnumerable<T> GetCustomAttributesDeep<T>(this MemberInfo memberInfo) where T : Attribute
        {
            foreach (var attribute in memberInfo.GetCustomAttributes<T>(true))
            {
                yield return attribute;
            }

            // If the declaring type is null don't search in the interfaces.
            if (memberInfo.DeclaringType == null)
            {
                yield break;
            }

            var interfaces = memberInfo.DeclaringType.GetInterfaces();

            // Check if we can get the member from the interface.
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Constructor:
                case MemberTypes.Custom:
                case MemberTypes.NestedType:
                case MemberTypes.Field:
                case MemberTypes.TypeInfo:
                    yield break;
            }

            // Find the attributes in the interfaces.
            foreach (var t in interfaces)
            {
                MemberInfo info;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Event:
                        info = t.GetEvent(memberInfo.Name);
                        break;
                    case MemberTypes.Method:
                        info = t.GetMethod(memberInfo.Name, ((MethodInfo)memberInfo).GetParameters().Select(p => p.ParameterType).ToArray());
                        break;
                    case MemberTypes.Property:
                        info = t.GetProperty(memberInfo.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (info == null)
                {
                    continue;
                }

                foreach (var attribute in info.GetCustomAttributes<T>(true))
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>
        ///     Get the first attribute from the the declaring type.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <returns>The first attribute.</returns>
        public static T GetCustomAttributeDeep<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return GetCustomAttributesDeep<T>(memberInfo).FirstOrDefault();
        }

        /// <summary>
        ///     Get the friendly name for the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The friendly name.</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (type == null)
                return "null";
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(long))
                return "long";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            return type.Name;
        }

        /// <summary>
        ///     Get the friendly name for the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="showParameters">True if the parameters should be included in the name.</param>
        /// <returns></returns>
        public static string GetFriendlyName(this MethodBase method, bool showParameters = true)
        {
            var str = method.Name;

            if (method.DeclaringType != null)
            {
                str = method.DeclaringType.GetFriendlyName() + '.' + str;
            }

            if (showParameters)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFriendlyName()));
                str += $"({parameters})";
            }

            return str;
        }

        /// <summary>
        ///     True if the type is a task.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTask(this Type type)
        {
            return typeof(Task).IsAssignableFrom(type);
        }
    }
}
