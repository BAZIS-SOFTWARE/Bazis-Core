using Geometry;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Project.Tasks.Functions.FrameFunctions
{
    public class EllipsBuilder
    {
        DoubleEllips elp;

        public EllipsBuilder()
        {
            elp = new DoubleEllips();
            elp.Name = FuncName.DEL.ToString();
        }

        public EllipsBuilder SetWidth(float width)
        {
            elp.Width = width;
            return this;
        }

        public EllipsBuilder SetDepth(float depth)
        {
            elp.Depth = depth;
            return this;
        }

        public EllipsBuilder SetFrontPart(float frontPart)
        {
            elp.FrontPart = frontPart;
            return this;
        }

        public EllipsBuilder SetBackPart(float backPart)
        {
            elp.BackPart = backPart;
            return this;
        }

        public static implicit operator DoubleEllips(EllipsBuilder builder)
        {
            return builder.elp;
        }
    }
}
