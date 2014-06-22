using System.Collections.Generic;

namespace PBT
{
    /// <summary>
    /// An abstract parent task. It holds an arbitrary number of subtasks.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public abstract class ParentTask<DataType> : Task<DataType>
	{
        /// <summary>
        /// All subtasks.
        /// </summary>
        public readonly Task<DataType>[] Subtasks;

        /// <summary>
        /// The subtask that is currently executed.
        /// </summary>
        public Task<DataType> CurrentTask;

        /// <summary>
        /// The constructor executed by a parent task implementation.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public ParentTask(TaskContext<DataType> context, Task<DataType>[] subtasks) : base(context)
		{
            this.CurrentTask = subtasks[0];
            this.Subtasks = subtasks;
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            if (CurrentTask != null && CurrentTask.Started)
                CurrentTask.SafeEnd();
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
			if(!CurrentTask.Started)
				CurrentTask.SafeStart();
			else if(CurrentTask.Finished)
			{
				CurrentTask.SafeEnd();
                ChildFinished();
			}
			else
				CurrentTask.DoAction();
		}

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public abstract void ChildFinished();
	}
}

