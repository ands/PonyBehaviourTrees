
namespace PBT.Decorators
{
	public class Filter<DataType> : TaskDecorator<DataType>
	{
        private Expression<bool> filter;

        public Filter(TaskContext<DataType> context, Task<DataType> task, Expression<bool> filter)
            : base(context, task)
		{
            this.filter = filter;
		}

        public override bool CheckConditions()
        {
            return filter && Subtask.CheckConditions();
        }

        public override void Start()
        {
            if(filter)
                base.Start();
            else
                Finish();
        }

        public override void DoAction()
        {
            if (filter)
                base.DoAction();
            else
                Finish();
        }
    }
}

