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
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        void Load(string path);

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
