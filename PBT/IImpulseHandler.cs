
namespace PBT
{
    /// <summary>
    /// The interface for impulse handlers.
    /// </summary>
    public interface IImpulseHandler
    {
        /// <summary>
        /// Will be executed when an impulse becomes active.
        /// </summary>
        /// <param name="handle">The impulse handle</param>
        void OnBeginImpulse(ImpulseHandle handle);

        /// <summary>
        /// Will be executed when an impulse becomes inactive.
        /// </summary>
        /// <param name="handle">The impulse handle</param>
        void OnEndImpulse(ImpulseHandle handle);
    }
}

