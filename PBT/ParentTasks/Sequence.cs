using System;
using PBT;

namespace PBT.ParentTasks
{
	public class Sequence<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;

        public Sequence(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        public override bool CheckConditions()
        {
            return Subtasks[0].CheckConditions();
        }
		
		public override void Start()
		{
            currentTaskIndex = 0;
            CurrentTask = Subtasks[0];
		}

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

