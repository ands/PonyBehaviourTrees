using System.Collections.Generic;

namespace PBT
{
	public abstract class ParentTask<DataType> : Task<DataType>
	{
        public readonly Task<DataType>[] Subtasks;
        public Task<DataType> CurrentTask;

        public ParentTask(TaskContext<DataType> context, Task<DataType>[] subtasks) : base(context)
		{
            this.Subtasks = subtasks;
		}

        public override void End()
        {
            if (CurrentTask != null && CurrentTask.Started)
                CurrentTask.SafeEnd();
        }
		
		public override void DoAction()
		{
            /*if (CurrentTask == null)
            {
                Context.Log.Error("CurrentTask is Null in {0}", this);
                Finish();
                return;
            }*/
			if(!CurrentTask.Started)
				CurrentTask.SafeStart();
			else if(CurrentTask.Finished)
			{
				CurrentTask.SafeEnd();
                ChildFinished();
			}
			else
				CurrentTask.DoAction();
		}

        public abstract void ChildFinished();
	}
}

