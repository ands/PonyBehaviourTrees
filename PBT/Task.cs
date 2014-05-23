
namespace PBT
{
	public abstract class Task<DataType>
	{
        public readonly TaskContext<DataType> Context;

#if DEBUG
        public bool Finished { get; private set; }
        public bool Started { get; private set; }
#else
        public bool Finished;
        public bool Started;
#endif

        public Task(TaskContext<DataType> context)
		{
			Context = context;
            Finished = false;
            Started = false;
		}

        public abstract bool CheckConditions();
		public abstract void Start();
		public abstract void End();
		public abstract void DoAction();
		
		public void SafeStart()
		{
            Finished = false;
			Started = true;
			Start();
		}
		
		public void SafeEnd()
		{
			End();
            Reset();
		}
		
		public void Finish()
		{
			Finished = true;
		}
		
		public virtual void Reset()
		{
			Finished = false;
            Started = false;
		}
	}
}

