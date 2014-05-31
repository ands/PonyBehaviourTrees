
namespace PBT
{
    /// <summary>
    /// An abstract decorator task. It wraps a single child task.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public abstract class TaskDecorator<DataType> : Task<DataType>
	{
        /// <summary>
        /// The wrapped subtask.
        /// </summary>
        public readonly Task<DataType> Subtask;

        /// <summary>
        /// The constructor executed by a decorator implementation.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
		public TaskDecorator(TaskContext<DataType> context, Task<DataType> task) : base(context)
		{
            Subtask = task;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
		public override bool CheckConditions()
		{
            return Subtask.CheckConditions();
		}

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            Subtask.SafeStart();
        }

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
		public override void End()
		{
            Subtask.SafeEnd();
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            if (Subtask.Finished)
                Finish();
            else
                Subtask.DoAction();
        }

        /// <summary>
        /// Reset the state.
        /// </summary>
        public override void Reset()
        {
            Subtask.Reset();
            base.Reset();
        }
	}
}

