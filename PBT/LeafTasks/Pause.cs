using System;
using PBT;

namespace PBT.LeafTasks
{
	public class Pause<DataType> : LeafTask<DataType>
	{
		private DateTime end;
        private Expression<uint> milliseconds;

        public Pause(TaskContext<DataType> context, Expression<uint> milliseconds)
            : base(context)
		{
			this.milliseconds = milliseconds;
		}

        public override bool CheckConditions()
        {
            return true;
        }
		
		public override void Start()
		{
			this.end = DateTime.Now.AddMilliseconds(milliseconds);
		}
		
		public override void End()
		{
		}
		
		public override void DoAction()
		{
			if(DateTime.Now >= end)
				Finish();
		}
    }
}

