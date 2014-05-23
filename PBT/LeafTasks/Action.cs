using System;

namespace PBT.LeafTasks
{
	public class Action<DataType> : LeafTask<DataType>
	{
		private Expression action;

        public Action(TaskContext<DataType> context, Expression action)
            : base(context)
		{
            this.action = action;
		}

        public override bool CheckConditions()
        {
            return true;
        }
		
		public override void Start()
		{
            Finish();
            action.Execute();
		}
		
		public override void End()
		{
		}
		
		public override void DoAction()
		{
		}
    }
}

