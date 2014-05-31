
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that ignores whether its child task is ready to run. This task is always ready and just skips its child task if it is not ready.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class IgnoreConditions<DataType> : TaskDecorator<DataType>
	{
        private bool conditions;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        public IgnoreConditions(TaskContext<DataType> context, Task<DataType> task)
            : base(context, task)
		{
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            conditions = Subtask.CheckConditions();
            return true;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            if (conditions)
                Subtask.SafeStart();
            else
                Finish();
        }

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            if (Started)
                Subtask.SafeEnd();
        }
	}
}

