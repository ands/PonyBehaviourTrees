using System;
using System.Collections.Generic;

namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that handles impulses. It runs its child task while the specified impulse is active (only one at a time).
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class DuringImpulse<DataType> : TaskDecorator<DataType>, IImpulseHandler
	{
        private List<ImpulseHandle> impulses = new List<ImpulseHandle>(2);
        private ImpulseHandle active;
        private string impulse;
        private Expression<string> sourceVariable;
        private Expression<string> eventVariable;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="impulse">The impulse.</param>
        /// <param name="sourceVariable">The name of the variable that will contain the impulse source.</param>
        /// <param name="eventVariable">The name of the variable that will contain the impulse event data.</param>
        public DuringImpulse(TaskContext<DataType> context, Task<DataType> task, Expression<string> impulse, Expression<string> sourceVariable, Expression<string> eventVariable)
            : base(context, task)
		{
            uint i = Convert.ToUInt32(Enum.Parse(context.ImpulseType, impulse));
            this.impulse = impulse;
            this.sourceVariable = sourceVariable;
            this.eventVariable = eventVariable;

            List<IImpulseHandler> handler;
            if (context.ImpulseHandler.TryGetValue(i, out handler))
                handler.Add(this);
            else
                context.ImpulseHandler.Add(i, new List<IImpulseHandler>() { this });
		}

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            if (active != null)
                return true;

            do
            {
                if (impulses.Count == 0)
                {
                    active = null;
                    return false;
                }

                active = impulses[0];
                impulses.Remove(active);
                string sourceName = sourceVariable;
                if (sourceName.Length > 0)
                    Context.Variables[sourceName] = active.Source;
                string eventName = eventVariable;
                if (eventName.Length > 0)
                    Context.Variables[eventName] = active.Data;
            } while (!Subtask.CheckConditions());

            return true;
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            if (!active.Active)
            {
                Finish();
                return;
            }

            base.Start();
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            if (!active.Active)
            {
                Finish();
                return;
            }

            base.DoAction();
        }

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            base.End();
            active = null;
        }

        /// <summary>
        /// IImpulseHandler implementation. Will be executed when an impulse becomes active.
        /// </summary>
        /// <param name="handle">The impulse handle</param>
        public void OnBeginImpulse(ImpulseHandle handle)
        {
            impulses.Add(handle);

            if (impulses.Count > 256)
                Context.Log.Warning("A DuringImpulse Task is accumulating a lot of unhandled impulses of type {0}", impulse);
        }

        /// <summary>
        /// IImpulseHandler implementation. Will be executed when an impulse becomes inactive.
        /// </summary>
        /// <param name="handle">The impulse handle</param>
        public void OnEndImpulse(ImpulseHandle handle)
        {
            impulses.Remove(handle);
        }
    }
}

