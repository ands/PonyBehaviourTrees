using System;

namespace PBTEditor.Data
{
    /// <summary>
    /// The data representation of a parameter from a task type.
    /// </summary>
	public class TaskTypeParameter
	{
        /// <summary>
        /// The name of the parameter.
        /// </summary>
		public string Name;

        /// <summary>
        /// The long type name of the parameter.
        /// </summary>
		public string Type;

        /// <summary>
        /// The short type name of the parameter.
        /// </summary>
		public string ShortType;

        /// <summary>
        /// The enum type if it is an enum parameter.
        /// </summary>
		public EnumType EnumType;

        /// <summary>
        /// Gets a value that indicates whether the parameter type is an enum or not. 
        /// </summary>
		public bool IsEnum { get { return EnumType != null; } }
		
        /// <summary>
        /// The task type parameter constructor.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter type name.</param>
		public TaskTypeParameter(string name, string type)
		{
			Name = name;
			Type = type;
			ShortType = Type.Substring(Type.LastIndexOf(".") + 1).TrimEnd(']');
		}
	}
}

