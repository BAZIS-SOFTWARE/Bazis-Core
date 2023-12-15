using System;

namespace ModelInterfaces
{
    public interface IModelLoader
    {
        IModelData Load(string path, float factor = 1);

        event Action<object, ILoaderEventArgs> LoadEvent;
    }
}
