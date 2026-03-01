using Project.Tasks.FrameCreators;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public enum FuncType
    {
        TF,
        PF,
        CPF
    }
    /// <summary>
    /// Function
    /// </summary>
    public abstract class Function
    {
        public List<Parameter> parameters = new List<Parameter>();
        /// <summary>
        /// FunctionType
        /// </summary>
        public FuncType FunctionType { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// CalcValue
        /// </summary>
        /// <returns></returns>
        public abstract double CalcValue();
/// <inheritdoc/>

        public override string ToString()
        {
            var name = Name.Contains("py") ? $"Custom={Name}" : Name;

            //var par = $"Par({string.Join(",", parameters)})";
            var par = string.Join(",", parameters);
            return $"{FunctionType},{name},{par}";
        }
    }
}
