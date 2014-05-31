
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that executes its child task as often as its ready to run.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class While<DataType> : TaskDecorator<DataType>
	{
        private bool conditions;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        public While(TaskContext<DataType> context, Task<DataType> task)
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

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
            if (!Subtask.Finished)
                Subtask.DoAction();
            else
            {
                Subtask.SafeEnd();
                if (Subtask.CheckConditions())
                    Subtask.SafeStart();
                else
                    Finish();
            }
		}
	}
}

