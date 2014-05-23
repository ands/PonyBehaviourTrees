using System;

namespace PBT
{
    public class ImpulseHandle
    {
        public readonly Enum Impulse;
        public readonly object Source;
        public readonly object Data;
        public bool Active { get; internal set; }

        internal IImpulseHandler[] Handler;

        internal ImpulseHandle(Enum impulse, object source, object data)
        {
            Impulse = impulse;
            Source = source;
            Data = data;
            Active = true;
        }
    }
}

