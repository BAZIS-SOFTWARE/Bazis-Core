using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Model.Utilities
{
    public class DefaultSetInfoComparer : IEqualityComparer<ISetInfo>
    {
/// <inheritdoc/>

        public bool Equals(ISetInfo x, ISetInfo y)
        {
            if (x.Name == y.Name)
                return true;
            else return false;
        }
/// <inheritdoc/>

        public int GetHashCode(ISetInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
