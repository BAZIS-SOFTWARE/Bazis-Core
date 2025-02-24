using System;

namespace PostProc.ScenePresenter
{
    public static class SwapValues
    {
        public static void Sort<T, V>(T p1, T p2, V p1V, V p2V, out V[] ndRes, out T[] ndCoords) where V : IComparable<V>
        {
            if (p1V.CompareTo(p2V) > 0)
            {
                ndRes = new V[] { p2V, p1V };
                ndCoords = new T[] { p2, p1 };
            }
            else
            {
                ndRes = new V[] { p1V, p2V };
                ndCoords = new T[] { p1, p2 };
            }
        }
    }
}
