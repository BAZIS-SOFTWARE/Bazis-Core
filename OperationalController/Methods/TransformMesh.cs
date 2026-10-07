using Geometry;
using Model.Interfaces;
using System;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void MoveMesh(ObjType objType, Point3D vector)
        {
            using (BeginChanges())
            {
                if (objType != ObjType.Узел && objType != ObjType.Точка)
                    throw new ArgumentException("Only nodes and points can be moved.", nameof(objType));
                if (vector._x == 0 && vector._y == 0 && vector._z == 0)
                    return;
                var objects = ModelData.ObjectData.GetObjects(objType);
                var changed = false;
                foreach (var item in objects)
                {
                    foreach (var coord in item.GetCoordinates())
                    {
                        var x = coord._x + vector._x;
                        var y = coord._y + vector._y;
                        var z = coord._z + vector._z;
                        if (coord._x == x && coord._y == y && coord._z == z)
                            continue;
                        coord._x = x;
                        coord._y = y;
                        coord._z = z;
                        changed = true;
                    }
                }
                if (changed)
                {
                    var sets = ModelData.ObjectData.GetSetsInfo(objType);
                    var set = sets.Single();
                    RegisterSetChange(set, ChangeAction.Updated);
                }
            }
        }

        /// <inheritdoc/>
        public void RotateMesh(ObjType objType, Point3D axis, float angle)
        {
            using (BeginChanges())
            {
                if (objType != ObjType.Узел && objType != ObjType.Точка)
                    throw new ArgumentException("Only nodes and points can be rotated.", nameof(objType));
                if (angle == 0)
                    return;
                var objects = ModelData.ObjectData.GetObjects(objType);
                var changed = false;
                foreach (var item in objects)
                {
                    foreach (var coord in item.GetCoordinates())
                    {
                        var rotated = Geometry.Vector.GetVectorRotation(coord, axis, angle);
                        if (coord._x == rotated._x && coord._y == rotated._y && coord._z == rotated._z)
                            continue;
                        coord._x = rotated._x;
                        coord._y = rotated._y;
                        coord._z = rotated._z;
                        changed = true;
                    }
                }
                if (changed)
                {
                    var sets = ModelData.ObjectData.GetSetsInfo(objType);
                    var set = sets.Single();
                    RegisterSetChange(set, ChangeAction.Updated);
                }
            }
        }
    }
}
