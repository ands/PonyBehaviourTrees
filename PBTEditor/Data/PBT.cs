using System.Text;
using System.Xml;

namespace PBTEditor.Data
{
    /// <summary>
    /// Static methods to work with the pbt type data representations.
    /// </summary>
	public static class PBT
	{
        /// <summary>
        /// Read task types from an xml reader.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <returns>Returns the task types read from the xml reader.</returns>
		public static TaskTypes GetTaskTypes(XmlTextReader reader)
		{
			TaskTypes tt = new TaskTypes();
			
			string section = "";
			while(reader.Read())
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;
				if(reader.Depth == 1)
					section = reader.Name;
				else if(reader.Depth > 1)
				{
					if(section == "Categories")
						tt.TaskTypeCategories.Add(new TaskTypeCategory(reader));
					else if(section == "Enums")
						tt.EnumTypes.Add(new EnumType(reader));
				}
			}
			
			foreach(TaskTypeCategory category in tt.TaskTypeCategories)
				foreach(TaskType type in category.TaskTypes)
					foreach(TaskTypeParameter param in type.Parameters)
						param.EnumType = tt.EnumTypes.Find(et => param.Type == et.Name);
			
			return tt;
		}
		
        /// <summary>
        /// Read task types from a file.
        /// </summary>
        /// <param name="filename">The file.</param>
        /// <returns>Returns the task types read from the file.</returns>
		public static TaskTypes GetTaskTypes(string filename)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			var tt = GetTaskTypes(reader);
			reader.Close();	
			return tt;
		}
		
        /// <summary>
        /// Serializes a pbt and writes it to an xml writer.
        /// </summary>
        /// <param name="writer">The xml writer.</param>
        /// <param name="task">The root task of the pbt.</param>
		public static void Serialize(XmlTextWriter writer, Task task)
		{
			task.Serialize(writer);
		}
		
        /// <summary>
        /// Serializes a pbt and writes it to a file.
        /// </summary>
        /// <param name="filename">The file.</param>
        /// <param name="task">The root task of the pbt.</param>
		public static void Serialize(string filename, Task task)
		{
			XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
			writer.WriteStartDocument();
			Serialize(writer, task);
			writer.WriteEndDocument();
			writer.Close();
		}

        /// <summary>
        /// Deserializes a pbt from an xml reader.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        /// <param name="types">The existing task types.</param>
        /// <returns>Returns the pbt root task.</returns>
		public static Task Deserialize(XmlTextReader reader, TaskTypes types)
		{
			return new Task(reader, types);
		}
		
        /// <summary>
        /// Deserializes a pbt from a file.
        /// </summary>
        /// <param name="filename">The file.</param>
        /// <param name="types">The existing task types.</param>
        /// <returns>Returns the pbt root task.</returns>
		public static Task Deserialize(string filename, TaskTypes types)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			var task = Deserialize(reader, types);
			reader.Close();
			return task;
		}
	}
}

