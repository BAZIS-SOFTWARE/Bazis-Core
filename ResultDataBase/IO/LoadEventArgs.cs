

namespace ResultDB.IO
{
    /// <summary>
    /// LoadEventArgs
    /// </summary>
    public class LoadEventArgs
    {
        /// <inheritdoc/>
        public string Message { get; }
        /// <summary>
        /// LoadEventArgs
        /// </summary>
        /// <param name="message"></param>
        public LoadEventArgs(string message)
        {
            Message = message;
        }
    }
}