using System;

namespace ModelInterfaces
{
    public interface IModelLoader
    {
        IModelData Load(string path);

        event Action<object, ILoaderEventArgs> LoadEvent;
    }
}
