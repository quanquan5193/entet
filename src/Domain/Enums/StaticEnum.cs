using System;
using System.Reflection;

namespace mrs.Domain.Enums
{
    public static class StaticEnum
    {
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static TEnum GetValueFromAttribute<TEnum, TAttribute>
           (string text, Func<TAttribute, string> valueFunc) where TAttribute : Attribute
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
                if (attribute != null)
                {
                    if (valueFunc.Invoke(attribute) == text)
                        return (TEnum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == text)
                        return (TEnum)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "text");
            // or return default(T);
        }
    }
}
