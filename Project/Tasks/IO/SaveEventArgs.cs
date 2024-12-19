using Project.Interfaces.IO;

namespace Project.Tasks.IO
{
    /// <summary>
    /// SaveEventArgs
    /// </summary>
    public class SaveEventArgs : ISaveEventArgs
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
