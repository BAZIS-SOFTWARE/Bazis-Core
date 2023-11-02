using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface IModelData
    {
        IObjectsData ObjectData { get; set; }
        IGroupData GroupData { get; set; }
        void Clear();

        void Load(IModelData modelData);

        void SetObjectData(IObjectsData objectData);

        
        /// <summary>
        /// Saver for model
        /// </summary>
        IModelSaver Saver { get; set; }
        
        /// <summary>
        /// Loader for model
        /// </summary>
        IModelLoader Loader { get; set; }
    }
}
