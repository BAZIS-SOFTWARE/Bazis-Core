using Geometry;
using Model.Interfaces;
using Model.ObjectsCollections;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void MoveMesh(ObjType objType, Point3D vector)
        {
            ModelData.ObjectData.Move(objType, vector);
            foreach (var setInfo in ModelData.ObjectData.GetSetsInfo(objType))
                PublishSetChange(setInfo, ChangeAction.Updated);
        }
        /// <inheritdoc/>
        public void RotateMesh(ObjType objType, Point3D axis, float angle)
        {
            ModelData.ObjectData.Rotate(objType, axis, angle);
            foreach (var setInfo in ModelData.ObjectData.GetSetsInfo(objType))
                PublishSetChange(setInfo, ChangeAction.Updated);
        }
    }
}
