using MaterialDB.FunctionData;
using MaterialDB.MaterialData;

namespace MaterialDB.Compatibility
{
    /// <summary>
    /// Выводит ArgumentKind для записей старых баз, где поле отсутствует.
    /// </summary>
    public interface IArgumentKindResolver
    {
        /// <summary>
        /// Заменяет Undefined у всех записей материала. Явно заданные значения не меняются.
        /// </summary>
        void Resolve(MaterialDBItem material);

        /// <summary>
        /// Заменяет Undefined у всех функций базы. Явно заданные значения не меняются.
        /// </summary>
        void Resolve(FunctionDBData functions);
    }
}
