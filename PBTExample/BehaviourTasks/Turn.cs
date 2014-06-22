using System;
using PBT;

namespace PBTExample
{
    /// <summary>
    /// A leaf task that turns the Actor. It is always ready to run.
    /// </summary>
	public class Turn : LeafTask<Actor>
	{
		private Expression<float> amount;
        private const float speed = 4.0f;
        private double startTime;
        private float startAngle;
        private float endAngle;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="amount">The amount expression.</param>
        public Turn(TaskContext<Actor> context, Expression<float> amount)
            : base(context)
		{
            this.amount = amount;
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            if (Started && !Finished)
                return false;
            return true;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
		public override void Start()
		{
            startTime = Context.Time;
            startAngle = Context.Data.Angle;
            endAngle = startAngle + amount;
		}

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
		public override void End()
		{
		}

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
		public override void DoAction()
		{
            float progress = (float)(Context.Time - startTime) * speed;
            if (progress >= 1.0f)
            {
                Context.Data.Angle = endAngle;
                Finish();
                return;
            }

            Context.Data.Angle = (1.0f - progress) * startAngle + progress * endAngle;
		}
    }
}

