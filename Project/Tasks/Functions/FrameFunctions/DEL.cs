using Geometry;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class DEL : FrameFunction
    {
        /// <inheritdoc/>
        public override float CalcMaxTimeStep(float velocity)
        {
            var length = (double)FrontPart + BackPart;
            if (!float.IsFinite(velocity) || velocity <= 0 || !double.IsFinite(length) || length <= 0)
                throw new ArgumentOutOfRangeException(nameof(velocity), "The source size and velocity must be positive and finite.");

            var step = length / (3d * velocity);
            if (!double.IsFinite(step) || step > float.MaxValue || step < float.Epsilon)
                throw new InvalidOperationException("The calculated source time step is outside the supported range.");

            return (float)step;
        }
        /// <summary>
        /// DEL
        /// </summary>
        public DEL()
        {
            Name = FuncName.DEL.ToString();
            FunctionType = FuncType.CPF;
            AddParameter(new Parameter("Width", ParameterType.Constant, 0));
            AddParameter(new Parameter("Depth", ParameterType.Constant, 0));
            AddParameter(new Parameter("FrontPart", ParameterType.Constant, 0));
            AddParameter(new Parameter("BackPart", ParameterType.Constant, 0));
            AddParameter(new Parameter("Pulse", ParameterType.Constant, 1));
        }

        /// <summary>
        /// Ширина сварочной ванны, мм
        /// </summary>
        public float Width { get; set; } // по оси X

        /// <summary>
        /// Глубина сварочной ванны, мм
        /// </summary>
        public float Depth { get; set; } // по оси Z

        /// <summary>
        /// Длина передней части сварочной ванны, мм
        /// </summary>
        public float FrontPart { get; set; } // по оси Y

        /// <summary>
        /// Длина хвостовой части сварочной ванны, мм
        /// </summary>
        public float BackPart { get; set; } // по оси Y

        //public Matrix<float> InverseTransitionMatrix { get; set; }

        private float MaxSemiaxis { get; set; }
        private float MinSemiaxis { get; set; }

        public override List<T> GetIntersectedObjects<T>(List<T> elements)
        {
            var insideElements = new List<T>();
            //DetermineInverseMatrix();
            MaxAndMinSemiaxis();

            foreach (var element in elements)
                if (IsPointInside(element.CalcCentr()))
                    insideElements.Add(element);

            return insideElements;
        }

        public void MaxAndMinSemiaxis()
        {
            var Max1 = Math.Max(Depth, Width / 2);
            var Min1 = Math.Min(Depth, Width / 2);
            MaxSemiaxis = Math.Max(Max1, BackPart);
            MinSemiaxis = Math.Min(Min1, FrontPart);
        }


        public bool IsPointInside(Point3D elementCenter)
        {
            var vectorLength = Vector.GetVectorLength(elementCenter);
            if (vectorLength <= MinSemiaxis) return true;
            else if (vectorLength > MaxSemiaxis) return false;

            var semiEllipsoid = GetPart(elementCenter);

            var result = Math.Pow(elementCenter._x / (Width / 2), 2) +
                Math.Pow(elementCenter._y / semiEllipsoid, 2) +
                Math.Pow(elementCenter._z / Depth, 2) - 1;

            return result <= 0;

        }

        private float GetPart(Point3D elementCenter)
        {
            float semiEllipsoid;
            if (elementCenter._y < 0)
                semiEllipsoid = BackPart;
            else semiEllipsoid = FrontPart;
            return semiEllipsoid;
        }

        public override bool IsOverlappingSelf(float velocity)
        {
            return velocity <= (FrontPart + BackPart) / 2;
        }

        public override double CalcValue(Point3D elementCenter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override double CalcValue()
        {
            double rad, halfBead;

            var x = (float)this["X"].GetValue();
            var y = (float)this["Y"].GetValue();
            var z = (float)this["Z"].GetValue();

            var elementCenter = new Point3D(x, y, z);

            if (IsPointInside(elementCenter))
            {
                // мощность берется за единицу

                var power = 1;
                var xx = elementCenter._x * elementCenter._x;
                var yy = elementCenter._y * elementCenter._y;
                var zz = elementCenter._z * elementCenter._z;

                var se = GetPart(elementCenter);

                var hw = Width / 2;

                var exp = Math.Exp(-(3 * xx / (hw * hw))) *
    Math.Exp(-(3 * yy / (se * se))) *
    Math.Exp(-(3 * zz / (Depth * Depth)));

                var powerDensity = 6 * Math.Sqrt(3) * 0.5 / hw / se/ Depth / (Math.PI * Math.Sqrt(Math.PI));

                return this["Pulse"].GetValue() * powerDensity * exp;

                // если есть зависимость от времени, то расчитать корректировку энергии
                // this["ENERG"].GetValue();
                //
                // Пример: var pulse = this["PULSE"].GetValue(); // так запись в строке  ...PULSE=Table(Термоцикл,TIME)...
                // return powerDensity * pulse;

            }
            return 0;
        }
    }
}
