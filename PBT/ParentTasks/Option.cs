using System;
using PBT;

namespace PBT.ParentTasks
{
	public class Option<DataType> : ParentTask<DataType>
	{
        public Option(TaskContext<DataType> context, Task<DataType>[] subtasks)
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
            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (Subtasks[i].CheckConditions())
                {
                    CurrentTask = Subtasks[i];
                    return;
                }
            }
		}

        public override void ChildFinished()
		{
			Finish();
		}
	}
}