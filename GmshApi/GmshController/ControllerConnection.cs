using GmshApi.GmshKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class ControllerConnection
    {
        /// <summary>
        /// Возвращает флаг инициализировано ли API
        /// </summary>
        /// <returns>Инициализировано ли API</returns>
        public static bool IsInitialized()
        {
            var ierr = 0;
            return KernelGeneral.gmshIsInitialized(ref ierr) != 0;
        }
        /// <summary>
        /// Инициализирует Gmsh Api
        /// </summary>
        public static void Initialize()
        {
            if (!IsInitialized())
            {
                var ierr = 0;
                String argv = String.Empty;
                KernelGeneral.gmshInitialize(0, ref argv, 0, 0, ref ierr);
            }
        }
        /// <summary>
        /// Завершает работу Gmsh Api
        /// </summary>
        public static void Finalize()
        {
            if (IsInitialized()) 
            {
                var ierr = 0;
                KernelGeneral.gmshFinalize(ref ierr);
            }
        }
    }
}
