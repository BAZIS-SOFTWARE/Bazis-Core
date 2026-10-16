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
        /// <summary>
        /// Размер пользовательского источника вдоль направления движения.
        /// </summary>
        public double MotionLength
        {
            get { return this["MotionLength"].GetValue(); }
            set { this["MotionLength"].SetValue(value); }
        }

        /// <inheritdoc/>
        public override float CalcMaxTimeStep(float velocity)
        {
            if (!float.IsFinite(velocity) || velocity <= 0 || !double.IsFinite(MotionLength) || MotionLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(velocity), "The source size and velocity must be positive and finite.");

            var step = MotionLength / (3d * velocity);
            if (!double.IsFinite(step) || step > float.MaxValue || step < float.Epsilon)
                throw new InvalidOperationException("The calculated source time step is outside the supported range.");

            return (float)step;
        }
        public CustomFrameFunction()
        {
            Name = FuncName.Custom.ToString();
            FunctionType = FuncType.CPF;
            AddParameter(new Parameter("MotionLength", ParameterType.Constant, 0));
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
