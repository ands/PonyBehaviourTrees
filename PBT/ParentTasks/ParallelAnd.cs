using System;
using PBT;

namespace PBT.ParentTasks
{
    /// <summary>
    /// A parent task that executes its subtasks in parallel. It completes when all subtasks have finished. It is ready if all subtasks are ready.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class ParallelAnd<DataType> : ParentTask<DataType>
	{
        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="subtasks">The children tasks.</param>
        public ParallelAnd(TaskContext<DataType> context, Task<DataType>[] subtasks)
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
                if (!Subtasks[i].CheckConditions())
                    return false;
            return true;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                Subtasks[i].SafeStart();
        }

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (Subtasks[i].Started)
                    Subtasks[i].SafeEnd();
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            for (int i = 0; i < Subtasks.Length; i++)
            {
                if (Subtasks[i].Finished)
                    ChildFinished();
                else
                    Subtasks[i].DoAction();
            }
        }

        /// <summary>
        /// Will be executed when a subtask finishes.
        /// </summary>
        public override void ChildFinished()
        {
            for (int i = 0; i < Subtasks.Length; i++)
                if (!Subtasks[i].Finished)
                    return;
            Finish();
        }
	}
}

