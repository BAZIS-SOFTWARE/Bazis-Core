using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Option
    {
        private delegate void gmshOptionSetNumberFun(string name, double value, ref int ierr);
        private gmshOptionSetNumberFun gmshOptionSetNumber = GetFunction<gmshOptionSetNumberFun>("gmshOptionSetNumber");

        private delegate void gmshOptionGetNumberFun(string name, ref double value, ref int ierr);
        private gmshOptionGetNumberFun gmshOptionGetNumber = GetFunction<gmshOptionGetNumberFun>("gmshOptionGetNumber");

        private delegate void gmshOptionSetStringFun(string name, string value, ref int ierr);
        private gmshOptionSetStringFun gmshOptionSetString = GetFunction<gmshOptionSetStringFun>("gmshOptionSetString");

        private delegate void gmshOptionGetStringFun(string name, ref IntPtr value, ref int ierr);
        private gmshOptionGetStringFun gmshOptionGetString = GetFunction<gmshOptionGetStringFun>("gmshOptionGetString");

        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="name">Строка в формате доступа к полю категории опций</param>
        /// <param name="value">Значение поля категории</param>
        public void SetNumber(string name, double value)
        {
            var ierr = 0;
            gmshOptionSetNumber(name, value, ref ierr);
        }
        /// <summary>
        /// Возвращает значение из поля категории опций
        /// </summary>
        /// <param name="name">Строка в формате доступа к полю категории опций</param>
        /// <returns>Значение поля категории опций</returns>
        public double GetNumber(string name)
        {
            var ierr = 0;
            double result = 0.0;
            gmshOptionGetNumber(name, ref result, ref ierr);
            return result;
        }
        /// <summary>
        /// Устанавливает строку параметров для категории опций
        /// </summary>
        /// <param name="name">Строка в формате доступа к полю категории опций</param>
        /// <param name="value">Значение поля категории</param>
        public void SetString(string name, string value)
        {
            var ierr = 0;
            gmshOptionSetString(name, value, ref ierr);
        }
        /// <summary>
        /// Возвращает строку параметров для категории опций
        /// </summary>
        /// <param name="name">Строка в формате доступа к полю категории опций</param>
        /// <returns>Строка параметров для категории опций</returns>
        public string GetString(string name)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshOptionGetString(name, ref intValue, ref ierr);
            return Memory.GetString(intValue);
        }

    }
}
