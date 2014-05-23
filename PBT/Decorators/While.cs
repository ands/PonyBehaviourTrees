
namespace PBT.Decorators
{
	public class While<DataType> : TaskDecorator<DataType>
	{
        public While(TaskContext<DataType> context, Task<DataType> task)
            : base(context, task)
		{
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

