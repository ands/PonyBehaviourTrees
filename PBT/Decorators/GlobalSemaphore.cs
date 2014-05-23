
namespace PBT.Decorators
{
    public class GlobalSemaphore<DataType> : TaskDecorator<DataType>
	{
        private Expression<string> identifier;
        private Expression<uint> concurrentExecutions;

        public GlobalSemaphore(TaskContext<DataType> context, Task<DataType> task, Expression<string> identifier, Expression<uint> concurrentExecutions)
            : base(context, task)
		{
            this.identifier = identifier;
            this.concurrentExecutions = concurrentExecutions;
		}

        public override bool CheckConditions()
        {
            return TaskContext<DataType>.GlobalSemaphores[identifier] < concurrentExecutions && Subtask.CheckConditions();
        }
		
		public override void Start()
		{
            TaskContext<DataType>.GlobalSemaphores[identifier]++;
            Subtask.SafeStart();
		}

        public override void End()
        {
            Subtask.SafeEnd();
            TaskContext<DataType>.GlobalSemaphores[identifier]--;
        }
    }
}

