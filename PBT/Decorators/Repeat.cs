
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that executes its child task the specified number of times until it completes.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Repeat<DataType> : TaskDecorator<DataType>
	{
        private Expression<uint> iterations;
		private uint iteration;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="iterations">The number of iterations that the child task should be executed.</param>
        public Repeat(TaskContext<DataType> context, Task<DataType> task, Expression<uint> iterations)
            : base(context, task)
		{
			this.iterations = iterations;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return iterations > 0 && Subtask.CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            Subtask.SafeStart();
			iteration = 0;
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
            Subtask.DoAction();
            if (Subtask.Finished)
			{
                if (++iteration < iterations)
                {
                    Subtask.Reset();
                    if (Subtask.CheckConditions())
                        Subtask.SafeStart();
                    else
                        Finish();
                }
                else
                    Finish();	
			}
		}
	}
}

