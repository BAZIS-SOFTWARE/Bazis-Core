using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace Project.Tasks.Functions
{
    internal class CustomFunction : Function
    {
        ScriptEngine engine;
        ScriptScope scope;

        string file;
        /// <summary>
        /// ExecutedFile
        /// </summary>
        public string ExecutedFile
        {
            get { return file; }
        }

        public override double CalcValue()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// CreateEngine
        /// </summary>
        /// <param name="file">Полный путь до исполняемого файла</param>
        public void CreateEngine(string file)
        {
            this.file = file;
            Name = file;

            string pythonCode = File.ReadAllText(file);

            engine = Python.CreateEngine();
            scope = engine.CreateScope();
            engine.Execute(pythonCode, scope);
        }

    }
}
