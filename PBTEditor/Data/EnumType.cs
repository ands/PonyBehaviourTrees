using System;
using System.Xml;

namespace PBTEditor.Data
{
    /// <summary>
    /// The data representation of an enum type.
    /// </summary>
	public class EnumType
	{
        /// <summary>
        /// The name of the enum type.
        /// </summary>
		public string Name;

        /// <summary>
        /// The enum value names.
        /// </summary>
		public string[] ValueNames;
		
		internal EnumType(XmlTextReader reader)
		{
			Name = reader.Name;
			ValueNames = reader.GetAttribute("names").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}

