
namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that completes immeadiately. It does nothing except for holding a string which can be seen in the editor.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class TODO<DataType> : LeafTask<DataType>
	{
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="TODO">The TODO text. Has no function.</param>
        public TODO(TaskContext<DataType> context, string TODO) : base(context)
		{
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
		public override bool CheckConditions()
		{
            return true;
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

