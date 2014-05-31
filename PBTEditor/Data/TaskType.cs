using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PBTEditor.Data
{
    /// <summary>
    /// The data representation of a task type.
    /// </summary>
	public class TaskType
	{
        /// <summary>
        /// Long type name for finding the correct type from all the loaded assemblies.
        /// </summary>
		public string TypeName;

        /// <summary>
        /// Short type name.
        /// </summary>
		public string Name;

        /// <summary>
        /// The parameters for this task type.
        /// </summary>
		public List<TaskTypeParameter> Parameters = new List<TaskTypeParameter>();

        /// <summary>
        /// The category that this task type belongs to.
        /// </summary>
		public TaskTypeCategory Category;
		
        /// <summary>
        /// Default parameter values for long type names.
        /// </summary>
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
		
        /// <summary>
        /// Creates an instance of this task type with the specified parameter values.
        /// </summary>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns>Returns a new instace of this type.</returns>
		public Task Create(params string[] parameterValues)
		{
			return new Task(this, null, parameterValues);
		}
		
        /// <summary>
        /// Creates an instance of this task type with default parameter values.
        /// </summary>
        /// <returns>Returns a new instace of this type.</returns>
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
		
        /// <summary>
        /// Shows the task constructor information.
        /// </summary>
        /// <returns>Returns the task constructor information.</returns>
		public override string ToString()
		{
			string paramString = string.Join(", ", Parameters.Select(p => p.Type + " " + p.Name).ToArray());
			return TypeName + "(" + paramString + ")";
		}
	}
}

