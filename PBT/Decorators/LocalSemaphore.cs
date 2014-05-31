
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that acts as a tree-local named critical section. It runs its child task if less than N other tree-local semaphores with the same identifier are active. (N = concurrent executions).
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class LocalSemaphore<DataType> : TaskDecorator<DataType>
	{
        private Expression<string> identifier;
        private Expression<uint> concurrentExecutions;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="identifier">The tree-local semaphore identifier to use.</param>
        /// <param name="concurrentExecutions">The number of allowed concurrent executions. Tasks that use the same identifier, have to specify the same amount of concurrent executions.</param>
        public LocalSemaphore(TaskContext<DataType> context, Task<DataType> task, Expression<string> identifier, Expression<uint> concurrentExecutions)
            : base(context, task)
		{
            this.identifier = identifier;
            this.concurrentExecutions = concurrentExecutions;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return Context.LocalSemaphores[identifier] < concurrentExecutions && Subtask.CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            Context.LocalSemaphores[identifier]++;
			Subtask.SafeStart();
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            Subtask.SafeEnd();
            Context.LocalSemaphores[identifier]--;
        }
    }
}

