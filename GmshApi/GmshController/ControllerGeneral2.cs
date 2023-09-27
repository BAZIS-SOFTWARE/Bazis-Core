using GmshApi.GmshKernel;
using System;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Предоставляет методы для перевода неуправляемой памяти в управляемую
    /// </summary>
    public static class ControllerGeneral2
    {
        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        public static void OptionSetNumber(string name, double value)
        {
            var ierr = 0;
            KernelGeneral.gmshOptionSetNumber(name, value, ref ierr);
        }
        /// <summary>
        /// Возвращает значение из поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <returns>Значение поля категории опций</returns>
        public static double OptionGetNumber(string name)
        {
            var ierr = 0;
            var result =  0.0;
            KernelGeneral.gmshOptionGetNumber(name, ref result, ref ierr);
            return result;
        }
        /// <summary>
        /// Задает набор значений для категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="values">[In]Значения строки</param>
        public static void OptionSetString(string name, string values)
        {
            var ierr = 0;
            KernelGeneral.gmshOptionSetString(name, values, ref ierr);
        }
    }
}