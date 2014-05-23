
namespace PBT.Decorators
{
	public class LocalSemaphore<DataType> : TaskDecorator<DataType>
	{
        private Expression<string> identifier;
        private Expression<uint> concurrentExecutions;

        public LocalSemaphore(TaskContext<DataType> context, Task<DataType> task, Expression<string> identifier, Expression<uint> concurrentExecutions)
            : base(context, task)
		{
            this.identifier = identifier;
            this.concurrentExecutions = concurrentExecutions;
		}

        public override bool CheckConditions()
        {
            return Context.LocalSemaphores[identifier] < concurrentExecutions && Subtask.CheckConditions();
        }
		
		public override void Start()
		{
            Context.LocalSemaphores[identifier]++;
			Subtask.SafeStart();
		}

        public override void End()
        {
            Subtask.SafeEnd();
            Context.LocalSemaphores[identifier]--;
        }
    }
}

