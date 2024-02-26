using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.IO
{
    /// <summary>
    /// SaveModelToSTLFile
    /// </summary>
    public class SaveModelToSTLFile : IModelSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaverEventArgs> SaveEvent;
/// <inheritdoc/>

        public void Save(IModelData model, string path)
        {
            throw new NotImplementedException();
        }
    }
}
