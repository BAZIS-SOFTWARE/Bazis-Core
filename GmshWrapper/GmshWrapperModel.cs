using System;
using System.Linq;
using System.Runtime.InteropServices;
using Model.GmshController;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Оборачивает основные методы модели полученные из GmshController
    /// </summary>
    public static class GmshWrapperModel
    {
        /// <summary>
        /// Возвращает весь список моделей в gmsh API
        /// </summary>
        /// <returns>Список моделей gmsh API</returns>
        public static string[] ModelList()
        {
            var ierr = 0;
            IntPtr names, names_n;
            names = names_n = IntPtr.Zero;
            GmshModel.gmshModelList(ref names, ref names_n, ref ierr);
            return GmshWrapperMemory.GetArrayOfNames(names, names_n);
        }
        /// <summary>
        /// Добавляет модель в список, делая ее текущей
        /// </summary>
        /// <param name="model">[In]Строка имя модели</param>
        public static void ModelAdd(string model)
        {
            var ierr = 0;
            GmshModel.gmshModelAdd(model, ref ierr);
        }
        /// <summary>
        /// Удаляет текущую модель из списка
        /// </summary>
        public static void ModelRemove()
        {
            var ierr = 0;
            GmshModel.gmshModelRemove(ref ierr);
        }
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <returns>Имя текущей модели</returns>
        public static string ModelGetCurrent()
        {
            var ierr = 0;
            IntPtr name = IntPtr.Zero;
            GmshModel.gmshModelGetCurrent(ref name, ref ierr);
            return GmshWrapperMemory.GetName(name);
        }
        /// <summary>
        /// Переключается на модель с текущим именем
        /// </summary>
        /// <param name="name">[In]Имя модели</param>
        /// <returns>true если модель найдена</returns>
        public static bool ModelSetCurrent(String name)
        {
            var ierr = 0;
            GmshModel.gmshModelSetCurrent(name, ref ierr);
            return ierr == 0;
        }
        /// <summary>
        /// Удаляет все модели
        /// </summary>
        public static void ModelClear()
        {
            var ierr = 0;
            GmshModel.gmshClear(ref ierr);
        }
        /// <summary>
        /// Находит модель в Gmsh Api
        /// </summary>
        /// <param name="name">[In]Имя модели</param>
        /// <returns>true если найдена</returns>
        public static bool ModelFind(String name) => ModelList().Where(v => v == name).Any();
        /// <summary>
        /// Удаляет модель по заданному имени
        /// </summary>
        /// <param name="name">[In]Имя модели</param>
        /// <returns>true если найдена и удалена</returns>
        public static bool ModelRemoveByName(String name)
        {
            var status = false;
            if (ModelFind(name))
            {
                status = true;
                ModelSetCurrent(name);
                ModelRemove();
            }
            return status;
        }
    }
}
