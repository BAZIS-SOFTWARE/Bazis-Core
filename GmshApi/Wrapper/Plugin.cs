using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Plugin
    {
        private delegate void gmshPluginSetNumberFun(string name, string option, double value, ref int ierr);
        private gmshPluginSetNumberFun gmshPluginSetNumber = GetFunction<gmshPluginSetNumberFun>("gmshPluginSetNumber");
        
        private delegate void gmshPluginSetStringFun(string name, string option, string value, ref int ierr);
        private gmshPluginSetStringFun gmshPluginSetString = GetFunction<gmshPluginSetStringFun>("gmshPluginSetString");
        
        private delegate int gmshPluginRunFun(string name, ref int ierr);
        private gmshPluginRunFun gmshPluginRun = GetFunction<gmshPluginRunFun>("gmshPluginRun");

        /// <summary>
        /// Установить значение для плагина
        /// </summary>
        /// <param name="name">Имя плагина</param>
        /// <param name="option">Имя опции</param>
        /// <param name="value">Значение опции</param>
        public void SetNumber(PluginName name, string option, double value)
        {
            var ierr = 0;
            gmshPluginSetNumber(name.ToString(), option, value, ref ierr);
        }
        /// <summary>
        /// Установить набор значений для плагина
        /// </summary>
        /// <param name="name">Имя плагина</param>
        /// <param name="option">Имя опции</param>
        /// <param name="value">Значения опции</param>
        public void SetString(PluginName name, string option, string value)
        {
            var ierr = 0;
            gmshPluginSetString(name.ToString(), option, value, ref ierr);
        }
        /// <summary>
        /// Запускает плагин с имененм name и возвращает идентификатор View(постпроцессинг) если имеется
        /// </summary>
        /// <param name="name">Имя плагина</param>
        /// <returns>Возращает объект View(по необходимости)</returns>
        public int Run(PluginName name)
        {
            var ierr = 0;
            return gmshPluginRun(name.ToString(), ref ierr);
        }
    }
}
