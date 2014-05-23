using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

namespace PBTEditor.Data
{
	public class Task
	{
		public TaskType TaskType;
		public string[] ParameterValues;
		public string Description;
		public List<Task> Subtasks;
		public bool Hidden = false;
		
		internal Task(TaskType type, string description = null, params string[] parameterValues)
		{
			TaskType = type;
			Description = description;
			ParameterValues = parameterValues;
			Subtasks = new List<Task>();
		}
		
		internal Task(XmlTextReader reader, TaskTypes types)
		{
			while(reader.Read())
			{
				if(reader.NodeType == XmlNodeType.Element)
				{
					Deserialize(reader, types);
					break;
				}
				else if(reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		
		internal void Serialize(XmlTextWriter writer)
		{
			writer.WriteStartElement(TaskType.TypeName);
			if(Description != null)
				writer.WriteAttributeString("description", Description);
			for(int i = 0; i < TaskType.Parameters.Count; i++)
				writer.WriteAttributeString(TaskType.Parameters[i].Name, ParameterValues[i]);
			foreach(var subtask in Subtasks)
				subtask.Serialize(writer);
			writer.WriteEndElement();
		}
		
		internal void Deserialize(XmlTextReader reader, TaskTypes types)
		{
			int depth = reader.Depth;
			
			// load type
			var typeName = reader.Name;
			TaskTypeCategory category = null;
			foreach(var c in types.TaskTypeCategories)
			{
				category = c;
				TaskType = c.TaskTypes.Find(type => type.TypeName == typeName);
				if(TaskType != null)
					break;
			}
			if(TaskType == null)
				throw new NotSupportedException("TaskType \"" + typeName + "\" not found.");
			
			// load parameter values
			ParameterValues = new string[TaskType.Parameters.Count];
			for(int i = 0; i < TaskType.Parameters.Count; i++)
				ParameterValues[i] = reader.GetAttribute(TaskType.Parameters[i].Name);
			Description = reader.GetAttribute("description");
			
			// load subtasks
			Subtasks = new List<Task>();
			if(category.Name == "Decorators" || category.Name == "ParentTasks")
			{
				while(true)
				{
					var subtask = new Task(reader, types);
					if(subtask.TaskType == null)
						break;
					Subtasks.Add(subtask);
				}
			}
			
			while(reader.Depth > depth)
				reader.Read();
		}
		
		public Task DeepCopy()
		{
			string[] p = new string[ParameterValues.Length];
			Array.Copy(ParameterValues, p, p.Length);
			var t = new Task(TaskType, Description, p);
			foreach(var s in Subtasks)
				t.Subtasks.Add(s.DeepCopy());
			return t;
		}
		
		public override string ToString()
		{
			string[] pv = new string[TaskType.Parameters.Count];
			for(int i = 0; i < TaskType.Parameters.Count; i++)
			{
				pv[i] = TaskType.Parameters[i].Type + " " + TaskType.Parameters[i].Name;
				pv[i] += " = " + ParameterValues[i];
			}
			return TaskType.TypeName + "(" + string.Join(", ", pv) + ")";
		}
	}
}

