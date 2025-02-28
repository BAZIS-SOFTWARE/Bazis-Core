using System;
using static GmshApi.GmshKernel;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Gmsh
    {
        //Инициализация
        private delegate void gmshInitializeFun(int argc, string[] argv, int readConfigFile, int run, ref int ierr);
        private gmshInitializeFun gmshInitialize = GetFunction<gmshInitializeFun>("gmshInitialize");

        private delegate int gmshIsInitializedFun(ref int ierr);
        private gmshIsInitializedFun gmshIsInitialized = GetFunction<gmshIsInitializedFun>("gmshIsInitialized");

        private delegate void gmshFinalizeFun(ref int ierr);
        private gmshFinalizeFun gmshFinalize = GetFunction<gmshFinalizeFun>("gmshFinalize");
        //Ввод\Вывод
        private delegate void gmshOpenFun(string fileName, ref int ierr);
        private gmshOpenFun gmshOpen = GetFunction<gmshOpenFun>("gmshOpen");

        private delegate void gmshMergeFun(string fileName, ref int ierr);
        private gmshMergeFun gmshMerge = GetFunction<gmshMergeFun>("gmshMerge");

        private delegate void gmshWriteFun(string fileName, ref int ierr);
        private gmshWriteFun gmshWrite = GetFunction<gmshWriteFun>("gmshWrite");
        //Очистка всех данных
        private delegate void gmshClearFun(ref int ierr);
        private gmshClearFun gmshClear = GetFunction<gmshClearFun>("gmshClear");
        
        /// <inheritdoc/>
        public Option Option { get; private set; }
        /// <inheritdoc/>
        public Model Model { get; private set; }
        /// <inheritdoc/>
        public Plugin Plugin { get; private set; }

        /// <inheritdoc/>
        public Logger Logger { get; private set; }

        /// <inheritdoc/>
        public Gmsh()
        {
            Option = new Option();
            Model = new Model();
            Plugin = new Plugin();
        }


        /// <summary>
        /// Инициализирует gmshApi
        /// </summary>
        /// <param name="argc">Количество строковых аргументов командной строки</param>
        /// <param name="argv">Массив строк аргуметов командной строки</param>
        /// <param name="readConfigFiles">Флаг - читать конфигурационный файл gmsh</param>
        /// <param name="run">Флаг запуска приложения из командной строке</param>
        public void Initialize(int argc = 0, string[] argv = null, bool readConfigFiles = true, bool run = false)
        {
            var ierr = 0;
            argv = argv ?? new string[0];
            gmshInitialize(argc, argv, Convert.ToInt32(readConfigFiles), Convert.ToInt32(run), ref ierr);
        }
        /// <summary>
        /// Возвращает флаг инициализации gmshApi
        /// </summary>
        /// <returns>1 - инциализированно, 0 - не инициализированно</returns>
        public int IsInitialized()
        {
            var ierr = 0;
            return gmshIsInitialized(ref ierr);
        }
        /// <summary>
        /// Деинициализирует gmshApi
        /// </summary>
        public void finalize()
        {
            var ierr = 0;
            gmshFinalize(ref ierr);
        }
        /// <summary>
        /// Открывает файл (.geo,step,.iges ...) и делает модель текущей
        /// </summary>
        /// <param name="fileName">Путь до файла</param>
        public void Open(string fileName)
        {
            var ierr = 0;
            gmshOpen(fileName, ref ierr);
        }
        /// <summary>
        /// Открывает файл (.geo,step,.iges ...) и делает слияние с текущей моделью
        /// </summary>
        /// <param name="fileName">Путь до файла</param>
        public void Merge(string fileName)
        {
            var ierr = 0;
            gmshMerge(fileName, ref ierr);
        }
        /// <summary>
        /// Записывает текущую модель в файл (.geo,step,.iges ...)
        /// </summary>
        /// <param name="fileName">Путь до файла</param>
        public void Write(string fileName)
        {
            var ierr = 0;
            gmshWrite(fileName, ref ierr);
        }
        /// <summary>
        /// Очищает пространсто gmshApi от всех моделей и данных постпроцессинга
        /// </summary>
        public void Clear()
        {
            var ierr = 0;
            gmshClear(ref ierr);
        }
    }
}
