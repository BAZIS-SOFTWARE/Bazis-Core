using System.Collections.Generic;

namespace ModelInterfaces.Comparers
{
    public class ElementSurfaceEqualityComparer : IEqualityComparer<ISurface>
    {
        public bool Equals(ISurface s1, ISurface s2)
        {
            if (s1.Equals(s2))
                return true;
            else
                return false;
        }

        public int GetHashCode(ISurface s)
        {
            return s.GetHashCode();
        }
    
    }
}
