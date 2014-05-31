using System;
using PBT;

namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that completes after the specified amount of time without doing anything.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Pause<DataType> : LeafTask<DataType>
	{
		private double end;
        private Expression<double> seconds;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="seconds">The amount of time to wait inside this task.</param>
        public Pause(TaskContext<DataType> context, Expression<double> seconds)
            : base(context)
		{
			this.seconds = seconds;
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
			end = Context.Time + seconds;
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
			if(Context.Time >= end)
				Finish();
		}
    }
}

