
namespace PBT.Decorators
{
	public class SlowDown<DataType> : TaskDecorator<DataType>
	{
		private double nextAction;
        private Expression<double> tickInterval;

        public SlowDown(TaskContext<DataType> context, Task<DataType> task, Expression<double> tickInterval)
            : base(context, task)
		{
            this.tickInterval = tickInterval;
		}

        public override bool CheckConditions()
        {
            return Subtask.CheckConditions();
        }
		
		public override void Start()
		{
            Subtask.SafeStart();
            this.nextAction = Context.Time + tickInterval;
		}

        public override void DoAction()
        {
            if (nextAction <= Context.Time)
            {
                nextAction = Context.Time + tickInterval;
                Subtask.DoAction();
                if (Subtask.Finished)
                    Finish();
            }
        }
	}
}

