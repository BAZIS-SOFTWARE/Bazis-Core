using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace Model.GroupsData
{
    public class StringNumberGroupComparer : IComparer<IGroup>
    {
        private string xName;
        private string yName;
        private int counter = 0;

        private int xLength { get { return xName.Length; } }
        private int yLength { get { return yName.Length; } }
        private bool isLastInX { get { return counter == xLength - 1; } }
        private bool isLastInY { get { return counter == yLength - 1; } }

        public int Compare(IGroup x, IGroup y)
        {
            if (x.Name == null || x.Name == string.Empty)
                return y.Name == null || y.Name == string.Empty ? 0 : -1;

            if (y.Name == null || y.Name == string.Empty)
                return x.Name == null || x.Name == string.Empty ? 0 : 1;

            xName = x.Name;
            yName = y.Name;

            if (xName.Length == 0 || yName.Length == 0)
                return xName.Length == 0 ? yName.Length == 0 ? 0 : -1 : 1;

            counter = 0;
            while (counter < xLength && counter < yLength)
            {
                var ASCIIx = (int)xName[counter];
                var ASCIIy = (int)yName[counter];
                int compare;

                if (ASCIIx == 95)
                    compare = ASCIIy == 95 ? 0 : -1;

                else if (CheckInBound(48, 57, ASCIIx))
                    compare = CompareDigit(ASCIIx, ASCIIy);

                else if (CheckInBound(65, 90, ASCIIx))
                    compare = CompareUpperLetter(ASCIIx, ASCIIy);

                else if (CheckInBound(97, 122, ASCIIx))
                    compare = CompareLowerLetter(ASCIIx, ASCIIy);

                else
                    compare = ASCIIx.CompareTo(ASCIIy);

                counter++;
                if (compare == 0)
                    continue;
                return compare;
            }
            var lenDif = xName.Length - yName.Length;
            return lenDif == 0 ? 0 : Math.Sign(lenDif);
        }

        private bool CheckInBound(int left, int right, int x) => x >= left && x <= right;

        private int CompareUpperLetter(int ASCIIx, int ASCIIy)
        {
            if (CheckInBound(48, 57, ASCIIy))
                return 1;

            if (CheckInBound(65, 90, ASCIIy))
                return ASCIIx.CompareTo(ASCIIy);

            if (CheckInBound(97, 122, ASCIIy))
            {
                var comp = ASCIIx - ASCIIy + 32;
                if (comp == 0)
                    return isLastInX ? -1 : 0;
                return Math.Sign(comp);
            }
            return 0;
        }

        private int CompareLowerLetter(int ASCIIx, int ASCIIy)
        {
            if (CheckInBound(48, 57, ASCIIy))
                return 1;

            if (CheckInBound(65, 90, ASCIIy))
            {
                var comp = ASCIIx - ASCIIy - 32;
                if (comp == 0)
                    return isLastInY ? 1 : 0;
                return Math.Sign(comp);
            }

            if (CheckInBound(97, 122, ASCIIy))
                return ASCIIx.CompareTo(ASCIIy);
            return 0;
        }

        private int CompareDigit(int ASCIIx, int ASCIIy)
        {
            if (CheckInBound(48, 57, ASCIIy))
            {
                var comp = ASCIIx.CompareTo(ASCIIy);
                if (isLastInX)
                    return isLastInY ? comp : char.IsDigit(yName[counter + 1]) ? -1 : comp;

                if (isLastInY)
                    return 1;

                if (xName[counter + 1] == '_')
                    return yName[counter + 1] == '_' ? 0 : -1;

                if (yName[counter + 1] == '_')
                    return xName[counter + 1] == '_' ? 0 : 1;
                return comp;
            }

            if (CheckInBound(65, 90, ASCIIy) || CheckInBound(97, 122, ASCIIy))
                return -1;
            return 1;
        }
    }
}
