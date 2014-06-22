using System;
using System.Collections.Generic;

namespace PBT.Decorators
{
    /// <summary>
    /// A decorator task that handles impulses. It executes its child task if the specified impulse was fired and not yet handled.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public class OnImpulse<DataType> : TaskDecorator<DataType>, IImpulseHandler
	{
        private List<ImpulseHandle> impulses = new List<ImpulseHandle>(2);
        private ImpulseHandle active;
        private string impulse;
        private uint impulseID;
        private Expression<string> sourceVariable;
        private Expression<string> dataVariable;

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="task">The child task.</param>
        /// <param name="impulse">The impulse.</param>
        /// <param name="sourceVariable">The name of the variable that will contain the impulse source.</param>
        /// <param name="dataVariable">The name of the variable that will contain the impulse data.</param>
        public OnImpulse(TaskContext<DataType> context, Task<DataType> task, Expression<string> impulse, Expression<string> sourceVariable, Expression<string> dataVariable)
            : base(context, task)
		{
            this.impulseID = Convert.ToUInt32(Enum.Parse(context.ImpulseType, impulse));
            this.impulse = impulse;
            this.sourceVariable = sourceVariable;
            this.dataVariable = dataVariable;

            List<IImpulseHandler> handler;
            if (context.ImpulseHandler.TryGetValue(impulseID, out handler))
                handler.Add(this);
            else
                context.ImpulseHandler.Add(impulseID, new List<IImpulseHandler>() { this });
		}

        /// <summary>
        /// The destructor.
        /// </summary>
        ~OnImpulse()
        {
            Context.ImpulseHandler[impulseID].Remove(this);
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
                string dataName = dataVariable;
                if (dataName.Length > 0)
                    Context.Variables[dataName] = active.Data;
            } while (!Subtask.CheckConditions());

            return true;
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
            for (int i = 0; i < impulses.Count; i++)
            {
                if (impulses[i].Impulse.Equals(handle.Impulse) &&
                    impulses[i].Source.Equals(handle.Source) && 
                    impulses[i].Data.Equals(handle.Data))
                {
                    impulses[i] = handle; // replace old identical impulse with new one... is this okay? :/
                    return; 
                }
            }

            impulses.Add(handle);

            if (impulses.Count > 256)
                Context.Log.Warning("An OnImpulse Task is accumulating a lot of unhandled impulses of type {0}", impulse);
        }

        /// <summary>
        /// IImpulseHandler implementation. Will be executed when an impulse becomes inactive.
        /// </summary>
        /// <param name="handle">The impulse handle</param>
        public void OnEndImpulse(ImpulseHandle handle)
        {
        }
    }
}

