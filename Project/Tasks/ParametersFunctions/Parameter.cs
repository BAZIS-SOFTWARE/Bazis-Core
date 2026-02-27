using MaterialDB.MaterialData;

namespace Project.Tasks.ParametersFunctions
{
    /// <summary>
    /// FunctionParameter
    /// </summary>
    public class Parameter<T>
    {
        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name} {Value}";
        }
    }
}