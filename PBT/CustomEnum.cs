using System;
using System.ComponentModel;
using System.Globalization;

namespace PBT
{
	public interface ICustomEnum
	{
        string Name { get; set; }
        int Value { get; set; }
	}

    public class CustomEnumConverter<T> : TypeConverter where T : ICustomEnum, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(int))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

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

