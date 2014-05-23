
namespace PBT.Decorators
{
	public class If<DataType> : TaskDecorator<DataType>
	{
        private Expression<bool> condition;

        public If(TaskContext<DataType> context, Task<DataType> task, Expression<bool> condition)
            : base(context, task)
		{
            this.condition = condition;
		}

        public override bool CheckConditions()
        {
            return condition && Subtask.CheckConditions();
        }
    }
}

