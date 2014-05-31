using System.Collections.Generic;

namespace PBTEditor.Data
{
    /// <summary>
    /// The data representation of all task types.
    /// </summary>
	public class TaskTypes
	{
        /// <summary>
        /// The list of task type categories.
        /// </summary>
		public List<TaskTypeCategory> TaskTypeCategories = new List<TaskTypeCategory>();

        /// <summary>
        /// The list of enum types.
        /// </summary>
		public List<EnumType> EnumTypes = new List<EnumType>();
	}
}

