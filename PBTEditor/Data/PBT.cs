using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace PBTEditor.Data
{
	public class TaskTypes
	{
		public List<TaskTypeCategory> TaskTypeCategories = new List<TaskTypeCategory>();
		public List<EnumType> EnumTypes = new List<EnumType>();
	}
	
	public static class PBT
	{
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
		
		public static TaskTypes GetTaskTypes(string filename)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			var tt = GetTaskTypes(reader);
			reader.Close();	
			return tt;
		}
		
		public static void Serialize(XmlTextWriter writer, Task task)
		{
			task.Serialize(writer);
		}
		
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

		public static Task Deserialize(XmlTextReader reader, TaskTypes types)
		{
			return new Task(reader, types);
		}
		
		public static Task Deserialize(string filename, TaskTypes types)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			var task = Deserialize(reader, types);
			reader.Close();
			return task;
		}
	}
}

