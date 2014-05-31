using System;
using PBT;

namespace PBT.ParentTasks
{
    /// <summary>
    /// A parent task that executes one random subtask that is ready. It is ready if at least one subtask is ready.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class PickRandom<DataType> : ParentTask<DataType>
	{
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public PickRandom(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (Subtasks[i].CheckConditions())
                    return true;
            return false;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            int[] permutation = new int[Subtasks.Length];
            for (int i = 0; i < Subtasks.Length; i++)
                permutation[i] = i;
            permutation.Shuffle(Context);

            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (Subtasks[permutation[i]].CheckConditions())
                {
                    CurrentTask = Subtasks[permutation[i]];
                    break;
                }
            }
		}

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public override void ChildFinished()
		{
			Finish();
		}
	}
}

