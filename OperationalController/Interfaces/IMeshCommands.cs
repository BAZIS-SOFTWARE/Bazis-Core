using Geometry;
using GmshApi;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System.Collections.Generic;
using System.Numerics;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции изменения и построения сеточной модели.
    /// </summary>
    public interface IMeshCommands
    {
        void MergeNodes(List<List<int>> numbers);
        void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs);
        void DeleteModelSet(ObjType objType, string name);
        void ClearModelCollection(ObjType objType);
        void ChangeMeshSetName(int elemDim, string oldName, string newName);
        void SetModelObjectsBackColor(ObjType objType);
        void ChangeInsideSurfacesState(bool state);
        void ChangeMeshSetOrder(int elemDim, string name, int order);
        void SetMeshMinimumSize(double size);
        void SetMeshMaximumSize(double size);
        void SetMeshAlgorithm2D(MeshAlgorithm2D algorithm);
        void SetMeshAlgorithm3D(MeshAlgorithm3D algorithm);
        void SetMeshSizeFactor(double factor);
        void SetCurveMeshingSettings(int curveNumber, CurveMeshingSettings settings);
        void SetSurfaceMeshingSettings(int surfaceNumber, SurfaceMeshingSettings settings);
        void SetVolumeMeshingSettings(int volumeNumber, VolumeMeshingSettings settings);
        void SetMeshPointSize(int pointNumber, double size);
        void RecombineMesh();
        void RefineMesh();
        void ClearGeometryMesh();
        void ClearGeometryMesh(int dimension);
        void GenerateMesh(int elemDim);
        void ConnectByBeams(string masterGroup, string slaveGroup, double radius, int maxBeams);
        void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements);
        void CreateElementsSet(string setName, int dim);
        void CreateElement(string setName, int dim, int[] nodeNumbers);
        void CreateNode(int number, float x, float y, float z);
        bool Create1DFrom2D();
        bool Create2DForm3D();
        IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes);
        void CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem);
        void HideMesh(int elemDim);
        void MergeElements(ObjType objType, string setMaster, string setSlave);
        void RenumberElements();
        void MoveMesh(ObjType objType, Point3D vector);
        void RotateMesh(ObjType objType, Point3D axis, float angle);
        string ExtrudeElement3DAlongCurve(int surfaceTag, int[] curveTags, int startPointTag, double step, bool transfinite);
        string ExtrudeElement3DAlongCurve(string setName, int[] curveTags, int startPointTag, double step, bool transfinite);
        string ExtrudeElement1DAlongCurve(int[] curveTags, int startPointTag, int startNodeTag, double step);
        string ExtrudeElement3DRotate(int surfaceTag, float angle, Vector3 originPoint, Vector3 rotAxis, bool transfinite);
        void ClearNotExistedModelData();
        void ClearEmptySet();
    }
}
