using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PBT;

namespace PBTInspector
{
    class PBTImpulseLogger<DataType, ImpulseType> : IImpulseHandler, IDisposable
    {
        public HashSet<ImpulseType> Enabled = new HashSet<ImpulseType>();

        private readonly Dictionary<uint, List<IImpulseHandler>> impulseHandler;
        private readonly TextWriter writer;

        public PBTImpulseLogger(TextWriter writer, TaskContext<DataType> context)
        {
            this.writer = writer;

            FieldInfo ImpulseHandlerField = context.GetType().GetField("ImpulseHandler", BindingFlags.NonPublic | BindingFlags.Instance);
            impulseHandler = (Dictionary<uint, List<IImpulseHandler>>)ImpulseHandlerField.GetValue(context);
            foreach (var ImpulseID in Enum.GetValues(typeof(ImpulseType)))
            {
                Enabled.Add((ImpulseType)Convert.ChangeType(ImpulseID, typeof(ImpulseType)));

                uint impulse = Convert.ToUInt32(ImpulseID);
                List<IImpulseHandler> handlers;
                if (impulseHandler.TryGetValue(impulse, out handlers))
                    handlers.Add(this);
                else
                    impulseHandler.Add(impulse, new List<IImpulseHandler>() { this });
            }
        }

        public void Dispose()
        {
            if (impulseHandler == null)
                return;
            foreach (var ImpulseID in Enum.GetValues(typeof(ImpulseType)))
                impulseHandler[Convert.ToUInt32(ImpulseID)].Remove(this);
        }

        public void OnBeginImpulse(ImpulseHandle handle)
        {
            if (Enabled.Contains((ImpulseType)Convert.ChangeType(handle.Impulse, typeof(ImpulseType))))
                writer.WriteLine("+ {0} {1}:\n  ({2}, {3})", handle.Impulse, handle.GetHashCode(), handle.Source, handle.Data);
        }

        public void OnEndImpulse(ImpulseHandle handle)
        {
            if (Enabled.Contains((ImpulseType)Convert.ChangeType(handle.Impulse, typeof(ImpulseType))))
                writer.WriteLine("- {0} {1}:\n  ({2}, {3})", handle.Impulse, handle.GetHashCode(), handle.Source, handle.Data);
        }
    }
}
