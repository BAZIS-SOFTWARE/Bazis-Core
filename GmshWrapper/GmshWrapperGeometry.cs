using System;
using Model.GmshController;
using Model.GmshUser;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class GmshWrapperGeometry
    {
        /// <summary>
        /// Возвращает набор геометрических сущностей Gmsh
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <returns>массив сущностей</returns>
        public static int[] GetEntities(int dim)
        {
            var ierr = 0;
            IntPtr dimTags, dimTags_n;
            dimTags = dimTags_n = IntPtr.Zero;
            var extension = GmshWrapperIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
               extension.Contains("brep") || extension.Contains("igs"))
                GmshGeometry.gmshModelGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            else
                GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            int[] tags;
            GmshWrapperMemory.GetTypesData<int>(dimTags,out tags, dimTags_n);
            return tags;
        }

        /// <summary>
        /// Возвращает имя сущности для заданной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Имя сущности</returns>
        public static string GetEntityName(int dim, int tag)
        {
            var error = 0;
            IntPtr name = IntPtr.Zero;
            GmshGeometry.gmshModelGetEntityName(dim, tag, ref name, ref error);
            return GmshUserMemory.GetName(name);
        }
        /// <summary>
        /// Получает физическое имя для данной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Физическое имя</returns>
        public static string GetPhysicalName(int dim, int tag)
        {
            var error = 0;
            IntPtr name = IntPtr.Zero;
            GmshGeometry.gmshModelGetPhysicalName(dim, tag, ref name ,ref error);
            return GmshUserMemory.GetName(name);
        }
        /// <summary>
        /// Возвращает размерность текущего геометрического объекта
        /// </summary>
        /// <returns>Размерность</returns>
        public static int GetDimension()
        {
            var ierr = 0;
            return GmshGeometry.gmshModelGetDimension(ref ierr);
        }
        /// <summary>
        /// Возвращает ограничивающий параллелепипед для указанного dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="min">[Out]Набор минимальных координат x,y,z</param>
        /// <param name="max">[Out]Набор максимальных координат x,y,z</param>
        public static void GetBoundingBox(int dim, int tag, out double[] min, out double[] max)
        {
            var ierr = 0;
            min = new double[3];
            max = new double[3];
            GmshGeometry.gmshModelGetBoundingBox(dim, tag, ref min[0], ref min[1], ref min[2],
                                                 ref max[0], ref max[1], ref max[2], ref ierr);
        }
    }
}
