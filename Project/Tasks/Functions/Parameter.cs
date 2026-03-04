using IronPython.Compiler.Ast;
using MaterialDB.MaterialData;
using Project.Tasks.Functions;
using System.Numerics;

namespace Project.Tasks.Functions
{
    public enum ParameterType
    {
        Variable,
        Constant
    }

    
    /// <summary>
    /// Parameter
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// ParameterType
        /// </summary>
        public ParameterType ParameterType { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Parameter(string name, ParameterType type,double value) : 
            this(name)
        {
            Value = value;
            ParameterType = type;
        }

        /// <summary>
        /// Parameter
        /// </summary>
        /// <param name="name"></param>

        public Parameter(string name)
        {
            Name = name;
        }



        /// <summary>
        /// Value
        /// </summary>
        protected double Value;

        /// <summary>
        /// GetValue
        /// </summary>
        /// <returns></returns>
        public virtual double GetValue()
        {
            return Value;
        }

        /// <summary>
        /// SetValue
        /// </summary>
        /// <returns></returns>
        public void SetValue(double value)
        {
                Value = value;
        }

        /// <inheritdoc/>

        public override string ToString()
        {
            var val = ParameterType == ParameterType.Variable ? "VAR" : Value.ToString();
            return $"{Name}={val}";
        }
    }

}