
using System;

namespace ModelInterfaces
{
    public interface IModelSaver
    {
        void Save(IModelData model, string path);

        event Action<object, ISaverEventArgs> SaveEvent;
    }
}
