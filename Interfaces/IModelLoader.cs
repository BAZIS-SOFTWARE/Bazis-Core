using Model;
using System;

namespace ModelInterface
{
    public interface IModelLoader
    {
        ModelData Load(string path);

        event Action<object, LoaderEventArgs> LoadEvent;
    }
}
