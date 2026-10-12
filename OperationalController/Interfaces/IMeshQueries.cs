using Geometry;
using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции чтения и анализа сеточной модели.
    /// </summary>
    public interface IMeshQueries
    {
        List<string> FindBoundaryEdges();
        Dictionary<int, List<int>> FindInterfacedNodes(List<int> nodes, int elemDim);
        Dictionary<int, List<int>> FindInterfacedNodes(int elemDim);
        List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color);
        List<int> SelectNodeInPlane(Plane plane, Color color);
        List<int> SelectE2DInPlane(float angleInGradus, int startNumber, Color color);
        IEnumerable<int> FindFreeNodes();
        SurfaceFigure GetSectionSurfaces(Plane plane);
        List<List<int>> FindCoincidentObjects(ObjType objType, float distance);
        Dictionary<int, Point3D> CalcNodeNormals();
        IEnumerable<Curve> CalcElemsNormals(int elemDim);

        IObjsPresenter CreateModelObjectsPresentor(ISetInfo setInfo);
        IEnumerable<ISetInfo> GetModelSetsInfo(ObjType objType);
        IEnumerable<ISetInfo> GetAllModelSetsInfo();
        ISetInfo GetModelSetInfo(ObjType objType, string setName);
        ISetInfo GetModelSetInfo(ObjType objType, int objNumber);
        MeshSettings GetMeshSettings();
        IEnumerable<Node> GetGeometryMeshNodes(int dimension, IEnumerable<int> entityNumbers, bool includeBoundary = false);
        CurveMeshingSettings GetCurveMeshingSettings(int curveNumber);
        SurfaceMeshingSettings GetSurfaceMeshingSettings(int surfaceNumber);
        VolumeMeshingSettings GetVolumeMeshingSettings(int volumeNumber);
        double GetMeshPointSize(int pointNumber);
        IModelObject GetModelObject(ObjType objType, int objNumber);
        IEnumerable<IModelObject> GetModelObjects(ObjType objType);
        IEnumerable<IModelObject> GetAllModelObjects();
        IEnumerable<Node> GetAllModelNodes();
        IEnumerable<IElement> GetAllModelElements();
        IEnumerable<IElement> GetModelElements(int dim, string setName);
        IEnumerable<ISurfaceElement> GetModelSurfaceElements(int dim);
        bool ContainsModelObject(ObjType objType, int objNumber);
    }
}
