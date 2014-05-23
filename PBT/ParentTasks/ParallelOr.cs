using System;
using PBT;

namespace PBT.ParentTasks
{
	public class ParallelOr<DataType> : ParentTask<DataType>
	{
        public ParallelOr(TaskContext<DataType> context, Task<DataType>[] subtasks)
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
                Subtasks[i].SafeStart();
        }

        public override void End()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (Subtasks[i].Started)
                    Subtasks[i].SafeEnd();
        }
			
		public override void DoAction()
		{
            for (int i = 0; i < Subtasks.Length; i++)
			{
				if (Subtasks[i].Finished)
                    ChildFinished();
				else
					Subtasks[i].DoAction();
			}
		}

        public override void ChildFinished()
        {
            Finish();
        }
	}
}

