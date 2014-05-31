
namespace PBT
{
    /// <summary>
    /// An abstract task.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public abstract class Task<DataType>
	{
        /// <summary>
        /// The execution context of the task.
        /// </summary>
        public readonly TaskContext<DataType> Context;

        /// <summary>
        /// Gets a value indicating whether the task has finished or not.
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task has been started or not.
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// The constructor executed by an implementation.
        /// </summary>
        /// <param name="context">The task context.</param>
        public Task(TaskContext<DataType> context)
		{
			Context = context;
            Finished = false;
            Started = false;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public abstract bool CheckConditions();

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public abstract void Start();

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
		public abstract void End();

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public abstract void DoAction();
		
        /// <summary>
        /// Put the task in a safe state and start it.
        /// </summary>
		public void SafeStart()
		{
            Finished = false;
			Started = true;
			Start();
		}
		
        /// <summary>
        /// Stop the task and put it in a safe state.
        /// </summary>
		public void SafeEnd()
		{
			End();
            Reset();
		}
		
        /// <summary>
        /// Finish the task.
        /// </summary>
		public void Finish()
		{
			Finished = true;
		}
		
        /// <summary>
        /// Reset the state.
        /// </summary>
		public virtual void Reset()
		{
			Finished = false;
            Started = false;
		}
	}
}

