using Geometry;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Project.Tasks.Functions
{
    internal class CustomFunction : FrameFunction
    {
        public string Path { get; }
        public CustomFunction(string path)
        {
            Path = path;

        }
        public override float CalcValue(Point3D elementCenter)
        {
            var localCoords = LocalFrame.Frame.GetCoordsInFrame(elementCenter);

            //TODO питон программа
            
            // 1. определяем внутри или снаружи объемной функции
            // на вход координаты точки в лск (центр объемной функции)
            // все геометрические параметры задаются в питон функции
            // примерный вид питон функции bool IsPointInside(double x,double y,double z)
            
            // 2. если внутри, то вычисляем значение. Если наоборот возвращаем ноль
            // примерный вид питон функции double CalcValue(double x,double y,double z)
            return 0;
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
