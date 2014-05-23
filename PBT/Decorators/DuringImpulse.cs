using System;
using System.Collections.Generic;

namespace PBT.Decorators
{
	public class DuringImpulse<DataType> : TaskDecorator<DataType>, IImpulseHandler
	{
        private List<ImpulseHandle> impulses = new List<ImpulseHandle>(2);
        private ImpulseHandle active;
        private string impulse;
        private Expression<string> sourceVariable;
        private Expression<string> eventVariable;

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

        public override void Start()
        {
            if (!active.Active)
            {
                Finish();
                return;
            }

            base.Start();
        }

        public override void DoAction()
        {
            if (!active.Active)
            {
                Finish();
                return;
            }

            base.DoAction();
        }

        public override void End()
        {
            base.End();
            active = null;
        }

        public void OnBeginImpulse(ImpulseHandle handle)
        {
            impulses.Add(handle);

            if (impulses.Count > 256)
                Context.Log.Warning("A DuringImpulse Task is accumulating a lot of unhandled impulses of type {0}", impulse);
        }

        public void OnEndImpulse(ImpulseHandle handle)
        {
            impulses.Remove(handle);
        }
    }
}

