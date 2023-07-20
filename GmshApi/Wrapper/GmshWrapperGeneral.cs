using GmshApi.Controller;
using System;

namespace GmshApi.Wrapper
{
    /// <summary>
    /// Предоставляет методы для перевода неуправляемой памяти в управляемую
    /// </summary>
    public static class GmshWrapperGeneral
    {
        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        public static void OptionSetNumber(String name, double value)
        {
            var ierr = 0;
            GmshGeneral.gmshOptionSetNumber(name, value, ref ierr);
        }
        /// <summary>
        /// Возвращает значение из поля категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <returns>Значение поля категории опций</returns>
        public static double OptionGetNumber(String name)
        {
            var ierr = 0;
            var result =  0.0;
            GmshGeneral.gmshOptionGetNumber(name, ref result, ref ierr);
            return result;
        }
        /// <summary>
        /// Задает набор значений для категории опций
        /// </summary>
        /// <param name="name">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="values">[In]Значения строки</param>
        public static void OptionSetString(String name, String values)
        {
            var ierr = 0;
            GmshGeneral.gmshOptionSetString(name, values, ref ierr);
        }
        /// <summary>
        /// Генерирует случайное число для типа Ulong
        /// </summary>
        /// <returns></returns>
        public static ulong GenerateRandom()
        {
            var dif = ulong.MaxValue - 1UL;
            var rand = new Random();
            var value = rand.NextDouble();
            return Convert.ToUInt64(value * dif);
        }
    }
}