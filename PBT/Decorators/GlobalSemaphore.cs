
namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that acts as a global named critical section. It runs its child task if less than N other global semaphores with the same identifier are active. (N = concurrent executions).
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class GlobalSemaphore<DataType> : TaskDecorator<DataType>
	{
        private Expression<string> identifier;
        private Expression<uint> concurrentExecutions;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="identifier">The global semaphore identifier to use.</param>
        /// <param name="concurrentExecutions">The number of allowed concurrent executions. Tasks that use the same identifier, have to specify the same amount of concurrent executions.</param>
        public GlobalSemaphore(TaskContext<DataType> context, Task<DataType> task, Expression<string> identifier, Expression<uint> concurrentExecutions)
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
            return TaskContext<DataType>.GlobalSemaphores[identifier] < concurrentExecutions && Subtask.CheckConditions();
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            TaskContext<DataType>.GlobalSemaphores[identifier]++;
            Subtask.SafeStart();
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            Subtask.SafeEnd();
            TaskContext<DataType>.GlobalSemaphores[identifier]--;
        }
    }
}

