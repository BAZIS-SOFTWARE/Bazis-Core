using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Functions
{
    public abstract class FrameFunction : IFrameFunction
    {
        public Frame Frame { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<T> GetIntersectedObjects<T>(List<T> elements) where T : IModelObject
        {
            throw new NotImplementedException();
        }

        public bool IsOverlappingSelf(float velocity)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return $"{Name};{Frame}";
        }
    }
}
