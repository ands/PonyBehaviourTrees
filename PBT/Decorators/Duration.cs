
namespace PBT.Decorators
{
	public class Duration<DataType> : TaskDecorator<DataType>
	{
        private double end;
        private Expression<double> seconds;

        public Duration(TaskContext<DataType> context, Task<DataType> task, Expression<double> seconds)
            : base(context, task)
		{
			this.seconds = seconds;
		}
		
		public override void Start()
		{
			Subtask.SafeStart();
			end = Context.Time + seconds;
		}
		
		public override void DoAction()
		{
			if(Subtask.Finished || Context.Time >= end)
				Finish();
			else
				Subtask.DoAction();
		}
	}
}

