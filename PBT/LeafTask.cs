
namespace PBT
{
	public abstract class LeafTask<DataType> : Task<DataType>
	{
		public LeafTask(TaskContext<DataType> context) : base(context)
		{
		}
	}
}