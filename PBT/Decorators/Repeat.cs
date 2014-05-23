
namespace PBT.Decorators
{
	public class Repeat<DataType> : TaskDecorator<DataType>
	{
        private Expression<uint> iterations;
		private uint iteration;

        public Repeat(TaskContext<DataType> context, Task<DataType> task, Expression<uint> iterations)
            : base(context, task)
		{
			this.iterations = iterations;
		}

        public override bool CheckConditions()
        {
            return iterations > 0 && Subtask.CheckConditions();
        }
		
		public override void Start()
		{
            Subtask.SafeStart();
			iteration = 0;
		}
		
		public override void DoAction()
		{
            Subtask.DoAction();
            if (Subtask.Finished)
			{
                if (++iteration < iterations)
                {
                    Subtask.Reset();
                    Subtask.SafeStart();
                }
                else
                    Finish();	
			}
		}
	}
}

