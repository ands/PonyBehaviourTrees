
namespace PBT.LeafTasks
{
    /// <summary>
    /// A leaf task that writes the specified string to the info logger.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class LogInfo<DataType> : LeafTask<DataType>
	{
        private Expression<string> text;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="text">The info text.</param>
        public LogInfo(TaskContext<DataType> context, Expression<string> text)
            : base(context)
		{
            this.text = text;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            return true;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            Finish();
            Log(text);
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
		public override void End()
		{
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
		}

        /// <summary>
        /// The logger.
        /// </summary>
        /// <param name="log">The text to log.</param>
        protected virtual void Log(string log)
        {
            Context.Log.Info(log);
        }
    }

    /// <summary>
    /// A leaf task that writes the specified string to the error logger.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class LogError<DataType> : LogInfo<DataType>
    {
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="text">The error text.</param>
        public LogError(TaskContext<DataType> context, Expression<string> text)
            : base(context, text)
        {
        }

        /// <summary>
        /// The logger.
        /// </summary>
        /// <param name="log">The text to log.</param>
        protected override void Log(string log)
        {
            Context.Log.Error(log);
        }
    }

    /// <summary>
    /// A leaf task that writes the specified string to the warning logger.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    public class LogWarning<DataType> : LogInfo<DataType>
    {
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="text">The warning text.</param>
        public LogWarning(TaskContext<DataType> context, Expression<string> text)
            : base(context, text)
        {
        }

        /// <summary>
        /// The logger.
        /// </summary>
        /// <param name="log">The text to log.</param>
        protected override void Log(string log)
        {
            Context.Log.Warning(log);
        }
    }
}

