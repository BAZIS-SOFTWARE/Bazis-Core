using Model.Events;
using System;

namespace Model.Interfaces
{
    public interface IModelSaver
    {
        void Save(ModelData model, string path);

        event Action<object, SaverEventArgs> SaveEvent;
    }
}
