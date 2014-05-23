using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PBT
{
	public class TaskContext<DataType>
	{
        public readonly DataType Data;
        public VariableStorage Variables = new VariableStorage();
        public readonly ILogger Log;
        public double Time { get; internal set; }
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

        public void OnEndImpulse(ImpulseHandle handle)
        {
            handle.Active = false;
            if (handle.Handler == null)
                return;
            foreach (IImpulseHandler h in handle.Handler)
                h.OnEndImpulse(handle);
        }

        public void OnImpulse(Enum impulse, object source = null, object eventObject = null)
        {
            if (impulse.GetType() != ImpulseType)
                throw new InvalidOperationException(impulse.ToString() + " is not of the type " + ImpulseType.ToString());

            ImpulseHandle handle = new ImpulseHandle(impulse, source, eventObject);
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

