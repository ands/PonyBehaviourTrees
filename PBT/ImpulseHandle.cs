using System;

namespace PBT
{
    /// <summary>
    /// A handle to identify a particular impulse occurence.
    /// </summary>
    public class ImpulseHandle
    {
        /// <summary>
        /// The impulse that occured.
        /// </summary>
        public readonly Enum Impulse;

        /// <summary>
        /// The source of the impulse.
        /// </summary>
        public readonly object Source;

        /// <summary>
        /// The passed data from the source.
        /// </summary>
        public readonly object Data;

        /// <summary>
        /// Gets a value that indicates whether the impulse is still active.
        /// </summary>
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

