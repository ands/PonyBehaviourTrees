using System;
using PBT;

namespace PBT.ParentTasks
{
    /// <summary>
    /// A parent task that is always executing the first (highest priority) subtask that is ready to run.
    /// Lower priority subtasks are stopped if a higher priority subtask becomes ready.
    /// It is ready if at least one subtask is ready.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class Prioritize<DataType> : ParentTask<DataType>
	{
        private int currentTaskIndex;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public Prioritize(TaskContext<DataType> context, Task<DataType>[] subtasks)
            : base(context, subtasks)
		{
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (Subtasks[i].CheckConditions())
                    return true;
            return false;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (Subtasks[i].CheckConditions())
                {
                    CurrentTask = Subtasks[i];
                    currentTaskIndex = i;
                    return;
                }
            }
            Finish();
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            if (CurrentTask == null)
            {
                Context.Log.Error("CurrentTask is Null in {0}", this);
                Finish();
                return;
            }

            // activate tasks with higher priority that are ready:
            for (int i = 0; i < currentTaskIndex; i++)
            {
                if (Subtasks[i].CheckConditions())
                {
                    if(CurrentTask.Started)
                        CurrentTask.SafeEnd();
                    CurrentTask = Subtasks[i];
                    currentTaskIndex = i;
                    break;
                }
            }

            base.DoAction();
        }

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public override void ChildFinished()
        {
            // give all tasks the chance to run:
            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (i != currentTaskIndex)
                {
                    if (Subtasks[i].CheckConditions())
                    {
                        CurrentTask = Subtasks[i];
                        currentTaskIndex = i;
                        return;
                    }
                }
            }
            Finish();
        }
	}
}

