using System;
using PBT;

namespace PBT.ParentTasks
{
	public class RandomOrder<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;
        private int[] permutation;
        private bool recalculatePermutation;

        public RandomOrder(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
            recalculatePermutation = true;
		}

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
		
		public override void Start()
		{
            currentTaskIndex = 0;
            CurrentTask = Subtasks[permutation[currentTaskIndex]];
            if (!CurrentTask.CheckConditions())
                Finish();
		}

        public override void End()
        {
            recalculatePermutation = true; // recalculate the permutation next time
        }

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

