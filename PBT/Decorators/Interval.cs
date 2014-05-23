
namespace PBT.Decorators
{
	public class Interval<DataType> : TaskDecorator<DataType>
	{
        private double nextAction;
        private Expression<double> seconds;

        public Interval(TaskContext<DataType> context, Task<DataType> task, Expression<double> seconds)
            : base(context, task)
		{
			this.seconds = seconds;
		}

        public override bool CheckConditions()
        {
            return nextAction <= Context.Time && Subtask.CheckConditions();
        }
		
		public override void Start()
		{
			Subtask.SafeStart();
			this.nextAction = Context.Time + seconds;
		}
	}
}