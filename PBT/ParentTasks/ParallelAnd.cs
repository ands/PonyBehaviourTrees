using System;
using PBT;

namespace PBT.ParentTasks
{
	public class ParallelAnd<DataType> : ParentTask<DataType>
	{
        public ParallelAnd(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        public override bool CheckConditions()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (!Subtasks[i].CheckConditions())
                    return false;
            return true;
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
            for (int i = 0; i < Subtasks.Length; i++)
                if (!Subtasks[i].Finished)
                    return;
            Finish();
        }
	}
}

