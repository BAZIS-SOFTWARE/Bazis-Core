using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using Model.GeometryObjects;
using MaterialDB.MaterialData;
using MaterialDB.FunctionData;
using Model.ObjectsCollections;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Data;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public bool ContainsConditions(string grName)
        {
            return TaskData.Contains(grName);
        }
        /// <inheritdoc/>
        public bool ContainsModelObject(ObjType objType, int objNumb)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return ModelData.ObjectData.PointsSet.ContainsKey(objNumb);
                case ObjType.Кривая:
                    return ModelData.ObjectData.CurveSet.ContainsKey(objNumb);
                case ObjType.Поверхность:
                    return ModelData.ObjectData.SurfaceSet.ContainsKey(objNumb);
                case ObjType.Узел:
                    return ModelData.ObjectData.NodesSet.ContainsKey(objNumb);
                case ObjType.Элемент1D:
                    return ModelData.ObjectData.E1DCollection.ContainsNumber(objNumb);
                case ObjType.Элемент2D:
                    return ModelData.ObjectData.E2DCollection.ContainsNumber(objNumb);
                default:
                    return ModelData.ObjectData.E3DCollection.ContainsNumber(objNumb);
            }
        }
    }
}
