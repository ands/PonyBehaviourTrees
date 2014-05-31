using System;
using PBT;

namespace PBT.ParentTasks
{
    /// <summary>
    /// A parent task that executes its subtasks in a random order. It is ready if the first subtask in the random order is ready to run.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class RandomOrder<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;
        private int[] permutation;
        private bool recalculatePermutation;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public RandomOrder(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
            recalculatePermutation = true;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            // CheckConditions may be executed multiple times to determine whether this task will run next
            if (recalculatePermutation)
            {
                permutation = new int[Subtasks.Length];
                for (int i = 0; i < Subtasks.Length; i++)
                    permutation[i] = i;
                permutation.Shuffle(Context);
                recalculatePermutation = false;
            }

            return Subtasks[permutation[0]].CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            currentTaskIndex = 0;
            CurrentTask = Subtasks[permutation[currentTaskIndex]];
            if (!CurrentTask.CheckConditions())
                Finish();
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            recalculatePermutation = true; // recalculate the permutation next time
        }

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public override void ChildFinished()
        {
            currentTaskIndex++;
            if (currentTaskIndex == Subtasks.Length || !Subtasks[permutation[currentTaskIndex]].CheckConditions())
                Finish();
            else
                CurrentTask = Subtasks[permutation[currentTaskIndex]];
        }
	}
}

