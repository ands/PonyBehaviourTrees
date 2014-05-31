
namespace PBT
{
    /// <summary>
    /// The logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Will be called in case of errors.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format args.</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// Will be called for verbose information.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format args.</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// Will be called in case of warnings.
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format args.</param>
        void Warning(string format, params object[] args);
	}
}

