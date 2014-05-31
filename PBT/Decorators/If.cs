
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that executes its child task if the specified condition expression evaluates to true.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class If<DataType> : TaskDecorator<DataType>
	{
        private Expression<bool> condition;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="condition">The condition expression. Return true to start the child task; otherwise, false.</param>
        public If(TaskContext<DataType> context, Task<DataType> task, Expression<bool> condition)
            : base(context, task)
		{
            this.condition = condition;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return condition && Subtask.CheckConditions();
        }
    }
}

