using System;
using static GmshApi.GmshController.GmshController;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Logger
    {
        private delegate void gmshLoggerStartFun(ref int ierr);
        private gmshLoggerStartFun gmshLoggerStart = GetFunction<gmshLoggerStartFun>("gmshLoggerStart");

        private delegate void gmshLoggerStopFun(ref int ierr);
        private gmshLoggerStopFun gmshLoggerStop = GetFunction<gmshLoggerStopFun>("gmshLoggerStop");

        private delegate void gmshLoggerGetFun(ref IntPtr log, ref IntPtr log_n, ref int ierr);
        private gmshLoggerGetFun gmshLoggerGet = GetFunction<gmshLoggerGetFun>("gmshLoggerGet");

        private delegate void gmshLoggerGetLastErrorFun(ref IntPtr error, ref int ierr);
        private gmshLoggerGetLastErrorFun gmshLoggerGetLastError = GetFunction<gmshLoggerGetLastErrorFun>("gmshLoggerGetLastError");

        /// <summary>
        /// Начинает запись в журнал
        /// </summary>
        public void Start()
        {
            var ierr = 0;
            gmshLoggerStart(ref ierr);
        }
        /// <summary>
        /// Останавливает запись в журнал
        /// </summary>
        public void Stop()
        {
            var ierr = 0;
            gmshLoggerStop(ref ierr);
        }
        /// <summary>
        /// Возвращает все записи журнала
        /// </summary>
        /// <returns>Записи журнала</returns>
        public string[] Get()
        {
            var ierr = 0;
            IntPtr log = IntPtr.Zero;
            IntPtr log_n = IntPtr.Zero;
            gmshLoggerGet(ref log, ref log_n, ref ierr);
            return GetArrayOfString(log, log_n);
        }
        /// <summary>
        /// Возвращает последнюю запись ошибки из журнала
        /// </summary>
        /// <returns>Запись ошибки</returns>
        public string GetLastError()
        {
            var ierr = 0;
            IntPtr error = IntPtr.Zero;
            gmshLoggerGetLastError(ref error, ref ierr);
            return GetString(error);
        }
    }
}
