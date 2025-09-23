using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IController
    {
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="path"></param>
        void Append(string path);


    }
}
