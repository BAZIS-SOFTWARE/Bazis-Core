using GmshApi.GmshKernel;
using System;
using System.IO;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Оборачивает основные методы IO полученные из GmshController
    /// </summary>
    public static class ControllerIO
    {
        /// <summary>
        /// Открывает файл с данными и делает новую модель текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла скрипта</param>
        public static void Open(string fileName)
        {
            var ierr = 0;
            KernelIO.gmshOpen(fileName, ref ierr);
        }
        /// <summary>
        /// Открывает файл с данными ассоциирует себя с текущей моделью
        /// </summary>
        /// <param name="fileName">[In]Путь до файла</param>
        public static void Load(string fileName)
        {
            var ierr = 0;
            KernelIO.gmshMerge(fileName, ref ierr);
        }
        /// <summary>
        /// Экспортирует данные модели в файл, расширение задает выходной формат файла
        /// </summary>
        /// <param name="fileName">Путь до файла</param>
        public static void Write(String fileName)
        {
            var ierr = 0;
            KernelIO.gmshWrite(fileName, ref ierr);
        }
        /// <summary>
        /// Возвращает имя файла ассоциированное с данной моделью
        /// </summary>
        /// <returns>Имя файла</returns>
        public static string GetCurrentModelFileName()
        {
            var ierr = 0;
            IntPtr fileName = IntPtr.Zero;
            KernelIO.gmshModelGetFileName(ref fileName, ref ierr);
            return ControllerMemory.GetName(fileName);
        }
        /// <summary>
        /// Задает имя файла для текущей модели
        /// </summary>
        /// <param name="fileName">[In]Имя файла</param>
        public static void SetCurrentModelFileName(string fileName)
        {
            var ierr = 0;
            KernelIO.gmshModelSetFileName(fileName, ref ierr);
        }
        /// <summary>
        /// Возвращает расширение файла привязанного к текущей модели
        /// </summary>
        /// <returns>Расширение файла</returns>
        public static string ModelGetFileNameExtension()
        {
            var fileName = ControllerIO.GetCurrentModelFileName();
            return Path.GetExtension(fileName);
        }
    }
}
