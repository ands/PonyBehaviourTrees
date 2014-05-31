using System.Collections.Generic;
using System.Xml;

namespace PBTEditor.Data
{
    /// <summary>
    /// The data representation of a category of tasks.
    /// </summary>
	public class TaskTypeCategory
	{
        /// <summary>
        /// The name of the category.
        /// </summary>
		public string Name;

        /// <summary>
        /// The task types that belong to this category.
        /// </summary>
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

