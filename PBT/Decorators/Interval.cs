
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that is only ready to run if at least the specified amount of time has passed since the last execution.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Interval<DataType> : TaskDecorator<DataType>
	{
        private double nextAction;
        private Expression<double> seconds;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="seconds">The amount of time between child task executions.</param>
        public Interval(TaskContext<DataType> context, Task<DataType> task, Expression<double> seconds)
            : base(context, task)
		{
			this.seconds = seconds;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return nextAction <= Context.Time && Subtask.CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
			Subtask.SafeStart();
			this.nextAction = Context.Time + seconds;
		}
	}
}