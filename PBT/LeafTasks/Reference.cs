using System;
using System.Collections.Generic;
using System.IO;

namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that is a reference to another pbt. It behaves like the specified pbt is a subtree that replaces this task.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class Reference<DataType> : RootTask<DataType>
	{
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="name">The name of the pbt file (without extension) in the current base path.</param>
        public Reference(TaskContext<DataType> context, Expression<string> name)
            : base(context, name)
        {

        }
	}
}

