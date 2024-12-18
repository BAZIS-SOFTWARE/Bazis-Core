using System.Collections.Generic;

namespace Model.Interfaces.Comparers
{
    /// <summary>
    /// ElementSurfaceEqualityComparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ElementSurfaceEqualityComparer<T> : IEqualityComparer<ISurface<T>>
        where T : IPoint
    {
        /// <inheritdoc/>
        public bool Equals(ISurface<T> s1, ISurface<T> s2)
        {
            if (s1.Equals(s2))
                return true;
            else
                return false;
        }
/// <inheritdoc/>

        public int GetHashCode(ISurface<T> s)
        {
            return s.GetHashCode();
        }
    
    }
}
