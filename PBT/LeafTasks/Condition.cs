
namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that is ready to run if the specified predicate expression returns true.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Condition<DataType> : LeafTask<DataType>
	{
		private Expression<bool> predicate;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="predicate">The predicate expression. Return true to allow executing this task; otherwise, false.</param>
        public Condition(TaskContext<DataType> context, Expression<bool> predicate)
            : base(context)
		{
            this.predicate = predicate;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            bool result = predicate;
            return result;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            Finish();
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
		public override void End()
		{
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
		}
    }
}

