
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that runs its child task whenever possible. It doesn't finish by itself.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Forever<DataType> : TaskDecorator<DataType>
	{
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        public Forever(TaskContext<DataType> context, Task<DataType> task)
            : base(context, task)
		{
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
            if (!Subtask.Started)
            {
                if (Subtask.CheckConditions())
                    Subtask.SafeStart();
            }
            else if (Subtask.Finished)
                Subtask.Reset();
            else
                Subtask.DoAction();
		}
	}
}

