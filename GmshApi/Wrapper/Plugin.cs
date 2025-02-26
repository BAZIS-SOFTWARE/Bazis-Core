using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Plugin
    {
        private delegate void gmshPluginSetNumberFun(string name, string option, double value, ref int ierr);
        private gmshPluginSetNumberFun gmshPluginSetNumber = GetFunction<gmshPluginSetNumberFun>("gmshPluginSetNumber");
        
        private delegate void gmshPluginSetStringFun(string name, string option, string value, ref int ierr);
        private gmshPluginSetStringFun gmshPluginSetString = GetFunction<gmshPluginSetStringFun>("gmshPluginSetString");
        
        private delegate int gmshPluginRunFun(string name, ref int ierr);
        private gmshPluginRunFun gmshPluginRun = GetFunction<gmshPluginRunFun>("gmshPluginRun");

        /// <inheritdoc/>
        public void PluginSetNumber(PluginName name, string option, double value)
        {
            var ierr = 0;
            gmshPluginSetNumber(name.ToString(), option, value, ref ierr);
        }
        /// <inheritdoc/>
        public void SetString(PluginName name, string option, string value)
        {
            var ierr = 0;
            gmshPluginSetString(name.ToString(), option, value, ref ierr);
        }
        /// <inheritdoc/>
        public int Run(PluginName name)
        {
            var ierr = 0;
            return gmshPluginRun(name.ToString(), ref ierr);
        }
    }
}
