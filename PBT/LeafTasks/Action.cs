using System;

namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that executes the specified expression. It is always ready to run.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Action<DataType> : LeafTask<DataType>
	{
		private Expression action;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="action">The action expression.</param>
        public Action(TaskContext<DataType> context, Expression action)
            : base(context)
		{
            this.action = action;
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
            action.Execute();
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

