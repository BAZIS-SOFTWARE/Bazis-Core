using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface IModelData
    {
        IObjectsData ObjectData { get; }
        IGroupData GroupData { get; }

        /// <summary>
        /// Clear all objects and groups
        /// </summary>
        void ClearAll();
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
        /// <summary>
        /// SetObjectData
        /// </summary>
        /// <param name="objectData"></param>
        void SetObjectData(IObjectsData objectData);
        /// <summary>
        /// SetGroupData
        /// </summary>
        /// <param name="objectData"></param>
        void SetGroupData(IGroupData objectData);

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
