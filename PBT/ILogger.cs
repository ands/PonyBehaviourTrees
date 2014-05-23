
namespace PBT
{
    public interface ILogger
    {
        void Error(string info, params object[] args);
        void Info(string info, params object[] args);
        void Warning(string info, params object[] args);
	}
}

