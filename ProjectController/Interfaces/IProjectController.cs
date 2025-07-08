using ModelControllerInterfaces.SurfaceMeshCreator;
using ModelControllerInterfaces.MeshObjsUtility;
using ModelControllerInterfaces.ObjsCreator;
using Model.Interfaces;
using ProjectController.MeshObjsUtility;
using Project.Tasks;
using Model;
using Project;
using Project.Interfaces;
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelController
    /// </summary>
    public interface IProjectController
    {
        /// <summary>
        /// FindBoundaryEdges
        /// </summary>
        /// <returns></returns>
        List<string> FindBoundaryEdges();
        /// <summary>
        /// TaskData
        /// </summary>
        ITaskData TaskData {get;}
        /// <summary>
        /// ModelData
        /// </summary>
        IModelData ModelData { get; }
        /// <summary>
        /// TaskData
        /// </summary>
        IGeneralData GeneralData { get; }
        /// <summary>
        /// Save
        /// </summary>
        void Save();
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="projectFile"></param>
        void Load(string projectFile);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        /// <param name="proj"></param>
        void Load(string path, string proj);
        /// <summary>
        /// InterfacedNodesFinder
        /// </summary>
        IObjectsRenumber ObjectsRenumber { get; }
        /// <summary>
        /// Extractor1DFrom2D
        /// </summary>
        IExtractor1DFrom2D Extractor1DFrom2D { get; }

        /// <summary>
        /// FreeNodesFinder
        /// </summary>
        IFreeNodesFinder FreeNodesFinder { get; }

        /// <summary>
        /// CrossSectionMaker
        /// </summary>
        ICrossSectionMaker CrossSectionMaker { get; }

        /// <summary>
        /// CoincidentObjectsFinder
        /// </summary>
        ICoincidentObjectsFinder CoincidentObjectsFinder { get; }

        /// <summary>
        /// ObjectsMerger
        /// </summary>
        IObjectsMerger ObjectsMerger { get; }
        /// <summary>
        /// DeloneMesh2DCreator. Creates 2D mesh use already existed nodes by Delone criterion. 
        /// </summary>
        ISurfaceMeshCreator SurfaceMeshCreator { get; }
        /// <summary>
        /// VolumeMesh3DCreator
        /// </summary>
        IVolumeMeshCreator VolumeMeshCreator { get; }
        /// <summary>
        /// NormalCalculator
        /// </summary>
        INormalCalculator NormalCalculator { get; }
    }
}
