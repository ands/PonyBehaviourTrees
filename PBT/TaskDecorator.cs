
namespace PBT
{
	public abstract class TaskDecorator<DataType> : Task<DataType>
	{
        public readonly Task<DataType> Subtask;
		
		public TaskDecorator(TaskContext<DataType> context, Task<DataType> task) : base(context)
		{
            Subtask = task;
		}

		public override bool CheckConditions()
		{
            return Subtask.CheckConditions();
		}
		
		public override void End()
		{
            Subtask.SafeEnd();
		}
		
		public override void Start()
		{
            Subtask.SafeStart();
		}

        public override void DoAction()
        {
            if (Subtask.Finished)
                Finish();
            else
                Subtask.DoAction();
        }

        public override void Reset()
        {
            Subtask.Reset();
            base.Reset();
        }
	}
}

