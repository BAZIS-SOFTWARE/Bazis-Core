using Project.Tasks.LocalFrames;
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
        PF,
        CPF
    }

    public enum FuncName
    {
        SPH,
        CIL,
        DEL,
        DHEX,
        FHEX,
        TT,
        PT,
        Custom
    }
    /// <summary>
    /// Function
    /// </summary>
    public abstract class Function
    {
        Dictionary<string,Parameter> parameters = new Dictionary<string, Parameter>();
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
            protected set;
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Parameter this[string key]
        {
            get { return parameters[key]; }
            set { parameters[key] = value; }
        }
        /// <summary>
        /// AddParameter
        /// </summary>
        /// <param name="parameter"></param>
        public void AddParameter(Parameter parameter)
        {
            if(!parameters.ContainsKey(parameter.Name))
                parameters.Add(parameter.Name, parameter);
        }
        /// <summary>
        /// ContainsParameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsParameter(string name)
        {
            return parameters.ContainsKey(name);
        }

        /// <summary>
        /// GetParameters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parameter> GetParameters()
        {
            foreach (var item in parameters)
                yield return item.Value;
        }

        /// <summary>
        /// GetParameters
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TableParameter> GetTableParameters()
        {
            foreach (var item in parameters)
                if (item.Value.ParameterKind == ParameterKind.Table)
                    yield return (TableParameter)item.Value;
        }

        /// <summary>
        /// CalcValue
        /// </summary>
        /// <returns></returns>
        public abstract double CalcValue();
/// <inheritdoc/>

        public override string ToString()
        {
            var par = string.Join(",", parameters.Values);

            return $"{Name},{par}";
        }
    }
}
