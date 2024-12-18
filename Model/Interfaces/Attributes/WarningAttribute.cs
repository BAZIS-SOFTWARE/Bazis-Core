using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Attributes
{
    /// <summary>
    /// WarningAttribute
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct |
        System.AttributeTargets.Method,
                       AllowMultiple = true)]
    public class WarningAttribute : System.Attribute
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// WarningAttribute
        /// </summary>
        /// <param name="message"></param>
        public WarningAttribute(string message)
        {
            Message = message;
        }
    }
}
