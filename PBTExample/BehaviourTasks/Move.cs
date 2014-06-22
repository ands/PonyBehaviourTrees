using System;
using PBT;

namespace PBTExample
{
    /// <summary>
    /// A leaf task that moves the Actor. It is always ready to run.
    /// </summary>
	public class Move : LeafTask<Actor>
	{
		private Expression<float> amount;
        private const float speed = 64.0f;
        private float toMove;
        private double lastFrameTime;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="amount">The amount expression.</param>
        public Move(TaskContext<Actor> context, Expression<float> amount)
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
            toMove = amount;
            lastFrameTime = Context.Time;
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
            float toMoveThisFrame = speed * (float)(Context.Time - lastFrameTime);
            lastFrameTime = Context.Time;
            
            // did we move enough?
            toMove -= toMoveThisFrame;
            if (toMove < 0.0f)
            {
                Finish();
                return;
            }

            float dx = (float)Math.Cos(Context.Data.Angle) * toMoveThisFrame;
            float dy = (float)Math.Sin(Context.Data.Angle) * toMoveThisFrame;

            // would the movement get us outside the world bounds?
            float x = Context.Data.X + dx;
            float y = Context.Data.Y + dy;
            var size = Context.Data.Simulation.WorldSize;
            if (x < 0 || x >= size.Width || y < 0 || y >= size.Height)
            {
                Context.OnImpulse(ActorImpulses.Leave, this, size);
                Finish();
                return;
            }

            // move!
            Context.Data.X = x;
            Context.Data.Y = y;
		}
    }
}

