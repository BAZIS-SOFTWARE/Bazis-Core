using Model;
using System;

namespace ModelInterface
{
    public interface IModelSaver
    {
        void Save(ModelData model, string path);

        event Action<object, SaverEventArgs> SaveEvent;
    }
}
