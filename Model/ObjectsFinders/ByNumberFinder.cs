using Model.Interfaces;
using System.Collections.Generic;

namespace Model.ObjectsFinders
{
    public static class ByNumberFinder
    {
        public static T Find<T>(List<T> objs, int objNumbr) where T : Interfaces.IModelObject
        {
            if (objs.Count - 1 == -1) { return default(T); }
            var left = 0;
            var right = objs.Count - 1;
            while (left < right)
            {
                var middle = (right + left) / 2;
                if (objNumbr <= objs[middle].Number)
                    right = middle;
                else left = middle + 1;
            }
            if (objs[right].Number == objNumbr)
                return objs[right];
            return default(T);
        }

        public static T Find<T>(T[] objs, int objNumbr) where T : Interfaces.IModelObject
        {
            if (objs.Length - 1 == -1) { return default(T); }
            var left = 0;
            var right = objs.Length - 1;
            while (left < right)
            {
                var middle = (right + left) / 2;
                if (objNumbr <= objs[middle].Number)
                    right = middle;
                else left = middle + 1;
            }
            if (objs[right].Number == objNumbr)
                return objs[right];
            return default(T);
        }
    }
}
