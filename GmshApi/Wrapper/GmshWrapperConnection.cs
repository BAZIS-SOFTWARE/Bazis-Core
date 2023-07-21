using System;
using System.Collections.Generic;
using System.Linq;
using GmshApi.Controller;

namespace GmshApi.GmshWrapper
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class GmshWrapperConnection
    {
        private static long globalIds = 1;
        private static List<HashSet<long>> users = new List<HashSet<long>>()
                                        { { new HashSet<long>() }, { new HashSet<long>() }};
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
        /// <summary>
        /// Возвращает свободный Id
        /// </summary>
        public static long GetId()
        {
            var result = 0L;
            if (users[0].Count == 0)
            {
                result = globalIds++;
                users[1].Add(result);
                return result;
            }
            result = users[0].First();
            users[0].Remove(result);
            return result;
        }
        /// <summary>
        /// Удаляет идентификатор
        /// </summary>
        /// <param name="id">Идентификатор для удаления</param>
        public static void RemoveId(long id)
        {
            users[1].Remove(id);
            users[0].Add(id);
        }
    }
}
