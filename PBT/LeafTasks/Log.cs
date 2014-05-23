
namespace PBT.LeafTasks
{
    public class LogInfo<DataType> : LeafTask<DataType>
	{
        private Expression<string> text;

        public LogInfo(TaskContext<DataType> context, Expression<string> text)
            : base(context)
		{
            this.text = text;
		}

        public override bool CheckConditions()
        {
            return true;
        }
		
		public override void Start()
		{
            Finish();
            Log(text);
		}
		
		public override void End()
		{
		}
		
		public override void DoAction()
		{
		}

        public virtual void Log(string log)
        {
            Context.Log.Info(log);
        }
    }

    public class LogError<DataType> : LogInfo<DataType>
    {
        public LogError(TaskContext<DataType> context, Expression<string> text)
            : base(context, text)
        {
        }

        public override void Log(string log)
        {
            Context.Log.Error(log);
        }
    }

    public class LogWarning<DataType> : LogInfo<DataType>
    {
        public LogWarning(TaskContext<DataType> context, Expression<string> text)
            : base(context, text)
        {
        }

        public override void Log(string log)
        {
            Context.Log.Warning(log);
        }
    }
}

