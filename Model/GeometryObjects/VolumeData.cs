using Model.GroupsData;
using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Model.GeometryObjects
{
    public class VolumeData : IList<VolumeFigure>
    {
        List<VolumeFigure> volumeFigures;
        /// <inheritdoc/>
        public VolumeFigure this[int index] 
        {
            get => volumeFigures[index];
            set => volumeFigures[index] = value;
        }
        /// <summary>
        /// VolumeData
        /// </summary>
        public VolumeData()
        {
            volumeFigures = new List<VolumeFigure>();
        }

        /// <inheritdoc/>
        public int Count => volumeFigures.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => false;
        /// <inheritdoc/>
        public void Add(VolumeFigure item)
        {
            volumeFigures.Add(item);
        }
        /// <inheritdoc/>
        public void Clear()
        {
            volumeFigures.Clear();
        }
/// <inheritdoc/>

        public bool Contains(VolumeFigure item)
        {
            return volumeFigures.Contains(item);
        }
        /// <inheritdoc/>
        public void CopyTo(VolumeFigure[] array, int arrayIndex)
        {
            volumeFigures.CopyTo(array, arrayIndex);
        }
        /// <inheritdoc/>
        public IEnumerator<VolumeFigure> GetEnumerator()
        {
            foreach (var item in volumeFigures)
            {
                yield return item;
            }
        }
        /// <inheritdoc/>
        public int IndexOf(VolumeFigure item)
        {
            return volumeFigures.IndexOf(item);
        }
        /// <inheritdoc/>
        public void Insert(int index, VolumeFigure item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(VolumeFigure item)
        {
            return volumeFigures.Remove(item);
        }

        public void RemoveAt(int index)
        {
            volumeFigures.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        internal void ClearNotExisted()
        {
            throw new NotImplementedException();
        }

    }
}
