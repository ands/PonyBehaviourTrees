using System;
using System.Xml;
using System.Collections.Generic;

namespace PBTEditor.Data
{
	public class TaskTypeCategory
	{
		public string Name;
		public List<TaskType> TaskTypes;
		
		internal TaskTypeCategory (XmlTextReader reader)
		{
			Name = reader.Name;
			TaskTypes = new List<TaskType>();
			
			if(!reader.IsEmptyElement)
			{
				while(reader.Read())
				{
					if(reader.NodeType == XmlNodeType.Element)
						TaskTypes.Add(new TaskType(reader, this));
					if(reader.NodeType == XmlNodeType.EndElement)
						break;
				}
			}
		}
	}
}

