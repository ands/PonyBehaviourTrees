using System;
using System.Xml;

namespace PBTEditor.Data
{
	public class EnumType
	{
		public string Name;
		public string[] ValueNames;
		
		internal EnumType(XmlTextReader reader)
		{
			Name = reader.Name;
			ValueNames = reader.GetAttribute("names").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}

