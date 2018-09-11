using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace VCD.Internal
{
    internal static class EnumExtensions
    {
        public static T FromEnumString<T>(this string enumString)
        {
            return (T)enumString.FromEnumString(typeof(T));
        }

        public static object FromEnumString(this string enumString, Type enumType)
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                EnumMemberAttribute enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetTypeInfo().GetDeclaredField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == enumString)
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException();
        }

        public static string ToEnumString<T>(this T enumValue)
        {
            return enumValue.ToEnumString(typeof(T));
        }

        public static string ToEnumString(this object enumValue, Type enumType)
        {
            string name = Enum.GetName(enumType, enumValue);
            EnumMemberAttribute enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetTypeInfo().GetDeclaredField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }
    }
}
