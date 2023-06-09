using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GMSHController
{
    /// <summary>
    /// Класс с группой методов для работы с файловой системой
    /// </summary>
    public static class GmshIO
    {
        /// <summary>
        /// Открывает скрипт файл .geo, .stp, .igs создает модель и делает ее текущей
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOpen(String fileName, ref int ierr);
        /// <summary>
        /// Открывает скрипт файл .geo, .stp, .igs ассоциирует данные с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshMerge(String fileName, ref int ierr);
        /// <summary>
        /// Экспортирует данные модели в файл, расширение задает выходной формат файла
        /// </summary>
        /// <param name="fileName">[In]Путь к файлу назначения</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshWrite(String fileName, ref int ierr);

        /// <summary>
        /// Получает путь до файла ассоциированный с данной моделью
        /// </summary>
        /// <param name="fileName">[Out]ANSI строка файла</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetFileName(ref IntPtr fileName, ref int ierr);

        /// <summary>
        /// Ассоциирует имя файла с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetFileName(String fileName, ref int ierr);

        /// <summary>
        /// Импортирует геометрию в файл в виде вектора пар размерность : идентификатор
        /// </summary>
        /// <param name="fileName">[In]Файл скрипта</param>
        /// <param name="outDimTags">[Out]Вектор пар размерность : идентификатор</param>
        /// <param name="outDimTags_n">[Out]Длина вектора пар</param>
        /// <param name="highestDimOnly">[In]Импортировать только верхнюю размерность</param>
        /// <param name="format">[In]Опциональная строка с расширением {brep,stp,iges}</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccImportShapes(String fileName, ref IntPtr outDimTags,
                                                           ref IntPtr outDimTags_n, int highestDimOnly,
                                                           String format, ref int ierr);
    }
}
