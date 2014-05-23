using System;
using PBT;

namespace PBT.ParentTasks
{
	public class Prioritize<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;

        public Prioritize(TaskContext<DataType> context, Task<DataType>[] subtasks)
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
                    currentTaskIndex = i;
                    return;
                }
            }
            Finish();
        }

        public override void DoAction()
        {
            if (CurrentTask == null)
            {
                Context.Log.Error("CurrentTask is Null in {0}", this);
                Finish();
                return;
            }

            // activate tasks with higher priority that are ready:
            for (int i = 0; i < currentTaskIndex; i++)
            {
                if (Subtasks[i].CheckConditions())
                {
                    if(CurrentTask.Started)
                        CurrentTask.SafeEnd();
                    CurrentTask = Subtasks[i];
                    currentTaskIndex = i;
                    break;
                }
            }

            base.DoAction();
        }

        public override void ChildFinished()
        {
            // give all tasks the chance to run:
            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (i != currentTaskIndex)
                {
                    if (Subtasks[i].CheckConditions())
                    {
                        CurrentTask = Subtasks[i];
                        currentTaskIndex = i;
                        return;
                    }
                }
            }
            Finish();
        }
	}
}

