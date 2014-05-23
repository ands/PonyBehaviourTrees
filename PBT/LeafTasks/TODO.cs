
namespace PBT.LeafTasks
{
    public class TODO<DataType> : LeafTask<DataType>
	{
        public TODO(TaskContext<DataType> context, string TODO) : base(context)
		{
		}
		
		public override bool CheckConditions()
		{
            return true;
		}
		
		public override void Start()
		{
            Finish();
		}

		public override void End()
		{
		}
		
		public override void DoAction()
		{
		}
	}
}

