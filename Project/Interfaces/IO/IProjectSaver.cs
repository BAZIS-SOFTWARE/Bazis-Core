using Model.Interfaces;
using System;

namespace Project.Interfaces.IO
{
    public interface IProjectSaver
    {

        event Action<object, ISaveEventArgs> SaveEvent;
        void Save(IProjectData project, string fullPath);

    }
}