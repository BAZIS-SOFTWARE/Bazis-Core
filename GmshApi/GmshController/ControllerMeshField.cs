using GmshApi.GmshKernel;
using System;


namespace GmshApi.GmshController
{
    public static class ControllerMeshField
    {
        /// <summary>
        /// Добавляет идентификатор и тип идентификатора в список полей
        /// </summary>
        /// <param name="fieldType">[In]Тип поля</param>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <returns>Идентификатор</returns>
        public static int Add(string fieldType, int tag)
        {
            int ierr = 0;
            return KernelMesh.gmshModelMeshFieldAdd(fieldType, tag, ref ierr);
        }
        /// <summary>
        /// Удаляет идентификатор из списка идентификаторов
        /// </summary>
        /// <param name="tag"></param>
        public static void Remove(int tag)
        {
            int ierr = 0;
            KernelMesh.gmshModelMeshFieldRemove(tag, ref ierr);
        }
        /// <summary>
        /// Возвращает список идентификаторов полей
        /// </summary>
        /// <returns>Список идентификаторов</returns>
        public static int[] List()
        {
            int ierr = 0;
            IntPtr tags = IntPtr.Zero;
            IntPtr tags_n = IntPtr.Zero;
            KernelMesh.gmshModelMeshFieldList(ref tags, ref tags_n, ref ierr);
            int[] tagsList;
            ControllerMemory.GetObjectsData<int>(tags, out tagsList, tags_n);
            return tagsList;
        }
        /// <summary>
        /// Возвращает тип поля по заданному идентификатору
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>тип поля</returns>
        public static string GetType(int tag)
        {
            int ierr = 0;
            IntPtr fieldType = IntPtr.Zero;
            KernelMesh.gmshModelMeshFieldGetType(tag, ref fieldType, ref ierr);
            return ControllerMemory.GetName(fieldType);
        }
        /// <summary>
        /// Задает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значение</param>
        public static void SetNumber(int tag, string option, double value)
        {
            int ierr = 0;
            KernelMesh.gmshModelMeshFieldSetNumber(tag, option, value, ref ierr);
        }
        /// <summary>
        /// Получает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <returns>Значение опции поля</returns>
        public static double GetNumber(int tag, string option)
        {
            var ierr = 0;
            var result = 0.0;
            KernelMesh.gmshModelMeshFieldGetNumber(tag, option, ref result, ref ierr);
            return result;
        }
        /// <summary>
        /// Задает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значения опции</param>
        public static void SetString(int tag, string option, string value)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshFieldSetString(tag, option, value, ref ierr);
        }
        /// <summary>
        /// Возвращает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <returns>Строка значений</returns>
        public static string GetString(int tag, string option)
        {
            var ierr = 0;
            IntPtr value = IntPtr.Zero;
            KernelMesh.gmshModelMeshFieldGetString(tag, option, ref value, ref ierr);
            return ControllerMemory.GetName(value);
        }
        /// <summary>
        /// Задает список значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="values">[In]Массив значений</param>
        public static void SetNumbers(int tag, string option, double[] values)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshFieldSetNumbers(tag, option, values, (IntPtr)values.Length, ref ierr);
        }
        /// <summary>
        /// Возвращает список значений для опции 
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <returns>Список значений опции</returns>
        public static double[] GetNumbers(int tag, string option)
        {
            var ierr = 0;
            IntPtr values = IntPtr.Zero;
            IntPtr values_n = IntPtr.Zero;  
            KernelMesh.gmshModelMeshFieldGetNumbers(tag, option, ref values, ref values_n, ref ierr);
            double[] valuesList;
            ControllerMemory.GetObjectsData(values, out valuesList, values_n);
            return valuesList;
        }
        /// <summary>
        /// Установить поле как BackgroundMesh
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        public static void SetAsBackgroundMesh(int tag)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshFieldSetAsBackgroundMesh(tag, ref ierr);
        }
        /// <summary>
        /// Установить поле как BoundaryLayer
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        public static void SetAsBoundaryLayer(int tag)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshFieldSetAsBoundaryLayer(tag, ref ierr);
        }
    }
}
