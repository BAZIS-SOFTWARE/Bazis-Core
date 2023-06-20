using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Model.GmshController;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс запросов пользователя в файловую систему
    /// </summary>
    public static class GmshUserIO
    {
        /// <summary>
        /// Получает путь до файла ассоциированного с данной моделью
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            var error = 0;
            IntPtr fileName = IntPtr.Zero;
            GmshIO.gmshModelGetFileName(ref fileName, ref error);
            var name = Marshal.PtrToStringAnsi(fileName);
            GmshGeneral.gmshFree(fileName);
            return name;
        }
    }
}
