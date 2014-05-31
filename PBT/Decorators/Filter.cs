
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that runs its child task as long as the specified filter expression returns true.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Filter<DataType> : TaskDecorator<DataType>
	{
        private Expression<bool> filter;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="filter">The filter expression. Return true while the child task execution is allowed; otherwise, false.</param>
        public Filter(TaskContext<DataType> context, Task<DataType> task, Expression<bool> filter)
            : base(context, task)
		{
            this.filter = filter;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return filter && Subtask.CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            if(filter)
                base.Start();
            else
                Finish();
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            if (filter)
                base.DoAction();
            else
                Finish();
        }
    }
}

