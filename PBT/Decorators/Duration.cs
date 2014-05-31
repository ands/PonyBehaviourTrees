
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that completes if the child task completes or the specified amount of time is elapsed.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Duration<DataType> : TaskDecorator<DataType>
	{
        private double end;
        private Expression<double> seconds;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="seconds">The amount of seconds after which the child task is stopped.</param>
        public Duration(TaskContext<DataType> context, Task<DataType> task, Expression<double> seconds)
            : base(context, task)
		{
			this.seconds = seconds;
		}
		
        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
			Subtask.SafeStart();
			end = Context.Time + seconds;
		}
		
        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
			if(Subtask.Finished || Context.Time >= end)
				Finish();
			else
				Subtask.DoAction();
		}
	}
}

