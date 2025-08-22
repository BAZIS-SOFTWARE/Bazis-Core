using Model;
using Newtonsoft.Json;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.Tasks;
using PropertiesCalculator.FunctionData;
using PropertiesCalculator.MaterialData;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace Project.IO
{
    /// <summary>
    /// LoadProjectFromTextFormat
    /// </summary>
    public class LoadProjectFromTextFormat : IProjectLoader
    {
        /// <inheritdoc/>
        public event Action<object, ILoadEventArgs> LoadEvent;
        /// <inheritdoc/>

        public void Load(IProjectData project, string fullPath)
        {
            Thread.Sleep(100);
            //modelData loading

            project.ModelData.Loader.LoadEvent += (ar1, ar2) =>
            { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };

            project.ModelData.Load(fullPath);          

            //taskData loading

            project.TaskData.Loader.LoadEvent += (ar1, ar2) =>
            { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };

            project.TaskData.Load(fullPath, project.ModelData.GroupData);
            
        }
    }
}
