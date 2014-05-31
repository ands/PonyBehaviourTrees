
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that executes its child task with a lower frame rate.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class SlowDown<DataType> : TaskDecorator<DataType>
	{
		private double nextAction;
        private Expression<double> tickInterval;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="tickInterval">The minimum allowed child frame interval in seconds.</param>
        public SlowDown(TaskContext<DataType> context, Task<DataType> task, Expression<double> tickInterval)
            : base(context, task)
		{
            this.tickInterval = tickInterval;
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
            this.nextAction = Context.Time + tickInterval;
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            if (nextAction <= Context.Time)
            {
                nextAction = Context.Time + tickInterval;
                Subtask.DoAction();
                if (Subtask.Finished)
                    Finish();
            }
        }
	}
}

