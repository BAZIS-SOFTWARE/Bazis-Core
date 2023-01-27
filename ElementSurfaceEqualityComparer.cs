using System.Collections.Generic;

namespace Model
{
    public class ElementSurfaceEqualityComparer : IEqualityComparer<Surface>
    {
        public bool Equals(Surface s1, Surface s2)
        {
            if (s1.Equals(s2))
                return true;
            else
                return false;
        }

        public int GetHashCode(Surface s)
        {
            return s.GetHashCode();
        }
    
    }
}
