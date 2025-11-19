using Geometry;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Project.Tasks.Functions
{
    public class CustomFunction : FrameFunction
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
        /// <summary>
        /// CustomFunction
        /// </summary>
        public CustomFunction()
        {
            Name = FunctionType.Custom;
        }


        /// <summary>
        /// CreateEngine
        /// </summary>
        /// <param name="file">Полный путь до исполняемого файла</param>
        public void CreateEngine(string file)
        {
            this.file = file;
            string pythonCode = File.ReadAllText(file);

            engine = Python.CreateEngine();
            scope = engine.CreateScope();
            engine.Execute(pythonCode, scope);
        }
/// <inheritdoc/>

        public override float CalcValue(Point3D elementCenter)
        {
            if (scope == null)
                throw new Exception($"Файл {file} не скомпилирована");

            var localCoords = LocalFrame.Frame.GetCoordsInFrame(elementCenter);

            var powerFunc = scope.GetVariable("CalcPower");
            return (float)powerFunc(localCoords._x, localCoords._y, localCoords._z);


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
    }
}
