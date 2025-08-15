

namespace ResultDB.IO
{
    /// <summary>
    /// SaveEventArgs
    /// </summary>
    public class SaveEventArgs
    {
        /// <summary>
        /// SaveEventArgs
        /// </summary>
        /// <param name="message"></param>
        public SaveEventArgs(string message)
        {
            Message = message;
        }
/// <inheritdoc/>

        public string Message { get; private set; }
    }
}
