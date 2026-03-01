using MaterialDB.MaterialData;
using Project.Tasks.Functions;
using System.Numerics;

namespace Project.Tasks.Functions
{
    public enum ParameterType
    {
        Constant,
        Variable
    }

    public enum ValueType
    {
        Digit,
        Table
    }

    
    /// <summary>
    /// Parameter
    /// </summary>
    public abstract class Parameter
    {
        /// <summary>
        /// ParameterType
        /// </summary>
        public ParameterType ParameterType { get; set; }
        /// <summary>
        /// ValueType
        /// </summary>
        public ValueType ValueType { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc/>

        public override string ToString()
        {
            var val = ParameterType == ParameterType.Variable ? "VAR" : Value;
            return $"{Name}={val}";
        }
    }

}