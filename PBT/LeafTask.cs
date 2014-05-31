
namespace PBT
{
    /// <summary>
    /// An abstract leaf task. It doesn't hold any subtasks.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public abstract class LeafTask<DataType> : Task<DataType>
	{
        /// <summary>
        /// The constructor executed by a leaf implementation.
        /// </summary>
        /// <param name="context">The task context.</param>
		public LeafTask(TaskContext<DataType> context) : base(context)
		{
		}
	}
}