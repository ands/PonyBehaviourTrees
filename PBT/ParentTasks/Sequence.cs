using System;
using PBT;

namespace PBT.ParentTasks
{
    /// <summary>
    /// A parent task that executes its subtasks one after another. It is ready if the first subtask is ready to run.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Sequence<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public Sequence(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return Subtasks[0].CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            currentTaskIndex = 0;
            CurrentTask = Subtasks[0];
		}

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public override void ChildFinished()
        {
            currentTaskIndex++;
            if (currentTaskIndex == Subtasks.Length || !Subtasks[currentTaskIndex].CheckConditions())
                Finish();
            else
                CurrentTask = Subtasks[currentTaskIndex];
        }
	}
}

