using ModelParcer.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
{
    public static class GmshUserConnection
    {
        private static long globalIds = 0;
        private static List<HashSet<long>> users = new List<HashSet<long>>()
                                        { { new HashSet<long>() }, { new HashSet<long>() }};
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

        /// <summary>
        /// Задает/Извлекает флаг инициализации GmshConnection
        /// </summary>
        public static bool IsInit { get; private set; }

        /// <summary>
        /// Инициализирует gmsh Api, если она не инициализирована
        /// </summary>
        public static void Init()
        {
            if (!IsInit)
            {
                int error = 0;
                var str = String.Empty;
                GmshConnection.gmshInitialize(0, ref str, 0, 0, ref error);
                IsInit = true;
            }
        }

        /// <summary>
        /// Завершает gmsh Api, если она инициализирована
        /// </summary>
        public static void Free()
        {
            if (IsInit) 
            {
                var error = 0;
                GmshModel.gmshClear(ref error);
                IsInit = false;
                GmshConnection.gmshFinalize(ref error);
            }
        }

    }
}
