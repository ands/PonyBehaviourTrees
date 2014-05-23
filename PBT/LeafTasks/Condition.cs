
namespace PBT.LeafTasks
{
	public class Condition<DataType> : LeafTask<DataType>
	{
		private Expression<bool> predicate;

        public Condition(TaskContext<DataType> context, Expression<bool> predicate)
            : base(context)
		{
            this.predicate = predicate;
		}

        public override bool CheckConditions()
        {
            bool result = predicate;
            return result;
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

