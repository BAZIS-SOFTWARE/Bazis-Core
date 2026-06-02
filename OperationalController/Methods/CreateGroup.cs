using Model.Interfaces;
using Model.MeshObjects;
using OperationalController.MeshObjsUtility;
using OperationalController.Methods.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using static IronPython.Modules._ast;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void CreateGroup(IEnumerable<IModelObject> objs)
        {
            ModelData.GroupData.Create(objs);
        }
        /// <inheritdoc/>
        public void CreateGroup(string grName, IEnumerable<IModelObject> objs)
        {

            ModelData.GroupData.Create(grName, objs);
        }

        /// <inheritdoc/>
        public string CreateGroupByMeshObjs(int dim, string setName)
        {  
            var objType = GetMeshObjType(dim);

            var objs = ModelData.ObjectData.GetObjects(objType, setName);
            return ModelData.GroupData.Create(objs);
        }

        private static ObjType GetMeshObjType(int dim)
        {
            ObjType objType;
            if (dim < 0 | dim > 3)
                throw new ObjectsDimensionException("Неправильно указана размерность элементов");

            if (dim == 1)
                objType = ObjType.Элемент1D;
            else if (dim == 2)
                objType = ObjType.Элемент2D;
            else if (dim == 3)
                objType = ObjType.Элемент3D;
            else
                objType = ObjType.Узел;
            return objType;
        }

        /// <inheritdoc/>
        public string CreateGroupByGeoObjs(int meshDim, int geoDim, int tag)
        {
            var numbs = new List<nint>();
            if(meshDim == 0)
                numbs = GmshController.Gmsh.Model.Mesh.GetNodes(geoDim, tag,true).Item1.ToList();
            else
            {
                foreach (var numb in GmshController.Gmsh.Model.Mesh.GetElements(geoDim, tag).Item2)
                    numbs.AddRange(numb);                
            }

            var objType = GetMeshObjType(meshDim);
            var objsList = new List<IModelObject>();
            foreach (var item in numbs)
                objsList.Add(ModelData.ObjectData.Find(objType, (int)item));  

            return ModelData.GroupData.Create(objsList);
        }

        /// <inheritdoc/>
        public string CreateOpenSurfaceNodesGroup(string setName)
        {
            if(ModelData.ObjectData.E3DCollection.Count == 0)
                throw new EmptyCollectionException("Пустая коллекция");

            var finder = new FindOpenSurfaceNodes();
            var surfaceNodes = finder.Find(ModelData.ObjectData.E3DCollection[setName].Values);

            var ndsList = surfaceNodes.Select(x => ModelData.ObjectData.NodesSet[x]);

            return ModelData.GroupData.Create(ndsList);
        }
    }
}
