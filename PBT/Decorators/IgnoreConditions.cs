
namespace PBT.Decorators
{
	public class IgnoreConditions<DataType> : TaskDecorator<DataType>
	{
        private bool conditions;

        public IgnoreConditions(TaskContext<DataType> context, Task<DataType> task)
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
	}
}

