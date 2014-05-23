using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Linq;

namespace PBTEditor.Data
{
	public class TaskTypeParameter
	{
		public string Name;
		public string Type;
		public string ShortType;
		public EnumType EnumType;
		public bool IsEnum { get { return EnumType != null; } }
		
		public TaskTypeParameter(string name, string type)
		{
			Name = name;
			Type = type;
			ShortType = Type.Substring(Type.LastIndexOf(".") + 1).TrimEnd(']');
		}
	}
	
	public class TaskType
	{
		public string TypeName;
		public string Name;
		public List<TaskTypeParameter> Parameters = new List<TaskTypeParameter>();
		public TaskTypeCategory Category;
		
		public static Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
		
		internal TaskType(XmlTextReader reader, TaskTypeCategory category)
		{
			Category = category;
			TypeName = reader.Name;
			Name = TypeName.Substring(TypeName.LastIndexOf('.') + 1);
			
			if(reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
					Parameters.Add(new TaskTypeParameter(reader.Name, reader.Value));
				reader.MoveToElement();
			}
		}
		
		public Task Create(params string[] parameterValues)
		{
			return new Task(this, null, parameterValues);
		}
		
		public Task Create()
		{
			var p = new string[Parameters.Count];
			for(int i = 0; i < p.Length; i++)
			{
				try
				{
					p[i] = Activator.CreateInstance(Type.GetType(Parameters[i].Type)).ToString();
				}
				catch
				{
					if(!DefaultValues.TryGetValue(Parameters[i].Type, out p[i]))
						p[i] = "";
				}
			}
			return new Task(this, null, p);
		}
		
		public override string ToString()
		{
			string paramString = string.Join(", ", Parameters.Select(p => p.Type + " " + p.Name).ToArray());
			return TypeName + "(" + paramString + ")";
		}
	}
}

