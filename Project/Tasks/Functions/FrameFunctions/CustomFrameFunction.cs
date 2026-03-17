using Geometry;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static IronPython.Modules._ast;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class CustomFrameFunction : FrameFunction
    {
        public CustomFrameFunction()
        {
            Name = FuncName.Custom.ToString();
        }
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

        /// <summary>
        /// CreateEngine
        /// </summary>

        public void CreateEngine(string file)
        {
            // потестировать!!!
            Name = file;

            string pythonCode = File.ReadAllText(file);

            engine = Python.CreateEngine();
            scope = engine.CreateScope();
            engine.Execute(pythonCode, scope);
        }

        /// <inheritdoc/>       
        public override double CalcValue(Point3D elementCenter)
        {
            if (scope == null)
                throw new Exception($"Файл {file} не скомпилирована");

            var powerFunc = scope.GetVariable("CalcPower");
            return (double)powerFunc(elementCenter._x, elementCenter._y, elementCenter._z);


            //TODO питон программа

            // 1. определяем внутри или снаружи объемной функции
            // на вход координаты точки в лск (центр объемной функции)
            // все геометрические параметры задаются в питон функции
            // примерный вид питон функции bool IsPointInside(double x,double y,double z)

            // 2. если внутри, то вычисляем значение. Если наоборот возвращаем ноль
            // примерный вид питон функции double CalcValue(double x,double y,double z)
        }

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            throw new NotImplementedException();
        }

        public override bool IsOverlappingSelf(float velocity)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override double CalcValue()
        {
            if (scope == null)
                throw new Exception($"Файл {file} не скомпилирована");

            var x = (float)this["X"].GetValue();
            var y = (float)this["Y"].GetValue();
            var z = (float)this["Z"].GetValue();

            var powerFunc = scope.GetVariable("CalcPower");
            return (double)powerFunc(x, y, z);
        }
    }
}
