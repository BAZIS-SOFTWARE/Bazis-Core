using Model.Interfaces;
using Model.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Model.Utilities
{
    public class DefaultModelObjsComparer<T> : IEqualityComparer<T>
        where T : IModelObject
    {
/// <inheritdoc/>

        public bool Equals(T x, T y)
        {
            if (x.Number == y.Number)
                return true;
            else return false;
        }
/// <inheritdoc/>

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(T obj)
        {
            return obj.Number;
        }
    }
}
