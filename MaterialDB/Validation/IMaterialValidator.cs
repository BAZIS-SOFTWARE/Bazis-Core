using MaterialDB.FunctionData;
using MaterialDB.MaterialData;

namespace MaterialDB.Validation
{
    /// <summary>
    /// Полная проверка структуры и полноты данных без исключений.
    /// </summary>
    public interface IMaterialValidator
    {
        /// <summary>
        /// Проверяет материал по правилам V1–V15.
        /// </summary>
        IEnumerable<ValidationIssue> Validate(MaterialDBItem material);

        /// <summary>
        /// Проверяет базу функций по правилам V1–V4.
        /// </summary>
        IEnumerable<ValidationIssue> Validate(FunctionDBData functions);
    }
}
