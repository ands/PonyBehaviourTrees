
namespace PBT
{
    public interface IImpulseHandler
    {
        void OnBeginImpulse(ImpulseHandle handle);
        void OnEndImpulse(ImpulseHandle handle);
    }
}

