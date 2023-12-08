
using System;

namespace ModelInterfaces
{
    /// <summary>
    /// IModelSaver
    /// </summary>
    public interface IModelSaver
    {
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        void Save(IModelData model, string path);
        /// <summary>
        /// SaveEvent
        /// </summary>
        event Action<object, ISaverEventArgs> SaveEvent;
    }
}
