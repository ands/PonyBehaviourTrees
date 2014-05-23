using System;
using PBT;

namespace PBT.ParentTasks
{
	public class PickRandom<DataType> : ParentTask<DataType>
	{
        public PickRandom(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        public override bool CheckConditions()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (Subtasks[i].CheckConditions())
                    return true;
            return false;
        }
		
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

        public override void ChildFinished()
		{
			Finish();
		}
	}
}

