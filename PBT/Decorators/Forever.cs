
namespace PBT.Decorators
{
	public class Forever<DataType> : TaskDecorator<DataType>
	{
        private bool conditions;

        public Forever(TaskContext<DataType> context, Task<DataType> task)
            : base(context, task)
		{
		}

        public override bool CheckConditions()
        {
            conditions = Subtask.CheckConditions();
            return true;
        }

        public override void Start()
        {
            if (conditions)
                Subtask.SafeStart();
            else
                Finish();
        }

        public override void End()
        {
            if (Started)
                Subtask.SafeEnd();
        }
		
		public override void DoAction()
		{
            if (!Subtask.Finished)
                Subtask.DoAction();
            else
            {
                Subtask.SafeEnd();
                if (Subtask.CheckConditions())
                    Subtask.SafeStart();
                else
                    Finish();
            }
		}
	}
}

