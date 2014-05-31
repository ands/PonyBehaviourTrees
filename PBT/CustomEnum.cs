using System;
using System.ComponentModel;
using System.Globalization;

namespace PBT
{
    /// <summary>
    /// Interface for creating task parameters that behave like enums in the pbt editor.
    /// Your custom enum will also need the [CustomEnumConverter&lt;YourCustomEnumType&gt;] attribute
    /// and a public static string[] Names property which returns the equivalent of Enum.GetNames(SomeEnumType).
    /// </summary>
	public interface ICustomEnum
	{
        /// <summary>
        /// The equivalent of an enum's name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The equivalent of an enum's numerical value.
        /// </summary>
        int Value { get; set; }
	}

    /// <summary>
    /// Converts implementations of ICustomEnum from strings (names) and ints (values).
    /// </summary>
    /// <typeparam name="T">The ICustomEnum implementation.</typeparam>
    public class CustomEnumConverter<T> : TypeConverter where T : ICustomEnum, new()
    {
        /// <summary>
        /// Returns true if this converter can perform the conversion; otherwise, false.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(int))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// The actual converter method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var customEnum = new T();
                customEnum.Name = (string)value;
                return customEnum;
            }
            else if (value is int)
            {
                var customEnum = new T();
                customEnum.Value = (int)value;
                return customEnum;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}

