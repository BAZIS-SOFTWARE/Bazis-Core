using System;
using System.Collections.Generic;
using System.Linq;

namespace GmshApi
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class GmshWrapperConnection
    {
        /// <summary>
        /// Возвращает флаг инициализировано ли API
        /// </summary>
        /// <returns>Инициализировано ли API</returns>
        public static bool IsInitialized()
        {
            var ierr = 0;
            return GmshConnection.gmshIsInitialized(ref ierr) != 0;
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
                GmshConnection.gmshInitialize(0, ref argv, 0, 0, ref ierr);
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
                GmshConnection.gmshFinalize(ref ierr);
            }
        }
    }
}
