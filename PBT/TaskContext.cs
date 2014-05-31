using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PBT
{
    /// <summary>
    /// The execution context of a task and/or pbt.
    /// </summary>
    /// <typeparam name="DataType">The type of the controlled entity.</typeparam>
	public class TaskContext<DataType>
	{
        /// <summary>
        /// The controlled entity.
        /// </summary>
        public readonly DataType Data;

        /// <summary>
        /// The storage for csharp scripting variables.
        /// </summary>
        public VariableStorage Variables = new VariableStorage();

        /// <summary>
        /// The logger instance.
        /// </summary>
        public readonly ILogger Log;

        /// <summary>
        /// The current game time in seconds.
        /// </summary>
        public double Time { get; internal set; }

        /// <summary>
        /// The source for random values.
        /// </summary>
        public readonly Random Random = new Random((int)Stopwatch.GetTimestamp());

        internal readonly Dictionary<string, uint> LocalSemaphores = new Dictionary<string, uint>(StringComparer.Ordinal);
        internal readonly static Dictionary<string, uint> GlobalSemaphores = new Dictionary<string, uint>(StringComparer.Ordinal);
        internal readonly Dictionary<uint, List<IImpulseHandler>> ImpulseHandler = new Dictionary<uint, List<IImpulseHandler>>();
        internal readonly Type ImpulseType;
        internal readonly string[] Usings;
        internal readonly string BaseDirectory;

        internal TaskContext(DataType data, Type impulseType, string[] usings, string baseDirectory, ILogger logger)
        {
            Log = logger;
            Data = data;
            Usings = usings;
            BaseDirectory = baseDirectory;

            if (!impulseType.IsEnum)
                throw new InvalidOperationException("impulseType needs to be the Type of an enum.");

            this.ImpulseType = impulseType;
        }

        internal void ResetForUpdate()
        {
            LocalSemaphores.Clear();
            // TODO: release global semaphores that were entered in the old tree.
            ImpulseHandler.Clear();
        }

        /// <summary>
        /// Lets the pbt handle an impulse.
        /// </summary>
        /// <param name="impulse">The impulse to handle.</param>
        /// <param name="source">The source of the impulse.</param>
        /// <param name="data">The passed data to handle the impulse.</param>
        /// <returns>Returns a handle for the impulse that is needed to end it.</returns>
        public ImpulseHandle OnBeginImpulse(Enum impulse, object source = null, object data = null)
        {
            if (impulse.GetType() != ImpulseType)
                throw new InvalidOperationException(impulse.ToString() + " is not of the type " + ImpulseType.ToString());

            ImpulseHandle handle = new ImpulseHandle(impulse, source, data);
            List<IImpulseHandler> handler;
            if (ImpulseHandler.TryGetValue(Convert.ToUInt32(impulse), out handler))
            {
                handle.Handler = handler.ToArray();

                foreach (IImpulseHandler h in handle.Handler)
                    h.OnBeginImpulse(handle);
            }
            return handle;
        }

        /// <summary>
        /// End the activity of the specified impulse.
        /// </summary>
        /// <param name="handle">The impulse handle.</param>
        public void OnEndImpulse(ImpulseHandle handle)
        {
            handle.Active = false;
            if (handle.Handler == null)
                return;
            foreach (IImpulseHandler h in handle.Handler)
                h.OnEndImpulse(handle);
        }

        /// <summary>
        /// Lets the pbt handle a short impulse.
        /// </summary>
        /// <param name="impulse">The impulse to handle.</param>
        /// <param name="source">The source of the impulse.</param>
        /// <param name="data">The passed data to handle the impulse.</param>
        public void OnImpulse(Enum impulse, object source = null, object data = null)
        {
            if (impulse.GetType() != ImpulseType)
                throw new InvalidOperationException(impulse.ToString() + " is not of the type " + ImpulseType.ToString());

            ImpulseHandle handle = new ImpulseHandle(impulse, source, data);
            List<IImpulseHandler> handler;
            if (ImpulseHandler.TryGetValue(Convert.ToUInt32(impulse), out handler))
            {
                handle.Handler = handler.ToArray();

                foreach (IImpulseHandler h in handle.Handler)
                    h.OnBeginImpulse(handle);

                handle.Active = false;

                foreach (IImpulseHandler h in handle.Handler)
                    h.OnEndImpulse(handle);
            }
        }
	}
}

