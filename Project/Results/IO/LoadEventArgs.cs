using Project.Interfaces.IO;

namespace Project.Results.IO
{
    /// <summary>
    /// LoadEventArgs
    /// </summary>
    public class LoadEventArgs : ILoadEventArgs
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