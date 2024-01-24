using ModelInterfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Model.GroupsData
{
    public class StringNumberGroupComparer : IComparer<IGroup>
    {
        public int Compare(IGroup x, IGroup y)
        {
            var xValues = SubstractNumericAndStringValues(x.GroupName);
            var yValues = SubstractNumericAndStringValues(y.GroupName);
            var isXBigger = xValues.Count > yValues.Count;
            for (var i = 0; i < Math.Min(xValues.Count, yValues.Count); i++)
            {
                var result = CompareStrings(xValues[i], yValues[i], i == Math.Min(xValues.Count, yValues.Count) - 1);
                if (result != 0 && xValues.Count == yValues.Count)
                    return result;
            }
            return isXBigger ? 1 : xValues.Count == yValues.Count ? 0 : -1;
        }

        private List<string> SubstractNumericAndStringValues(string groupName)
        {
            if (groupName.Length == 0)
                throw new ArgumentNullException("Имя группы не может быть пустым");
            var isDigitSelected = char.IsDigit(groupName[0]);
            var isStringSelected = !isDigitSelected;
            var sb = new StringBuilder();

            var result = new List<string>();
            foreach (var letter in groupName)
            {
                if (char.IsDigit(letter))
                {
                    if (isDigitSelected)
                        sb.Append(letter);
                    else
                    {
                        isDigitSelected = true;
                        isStringSelected = false;
                        result.Add(sb.ToString());
                        sb.Clear();
                        sb.Append(letter);
                    }
                    continue;
                }

                else
                {
                    if (isStringSelected)
                        sb.Append(letter);
                    else
                    {
                        isDigitSelected = false;
                        isStringSelected = true;
                        result.Add(sb.ToString());
                        sb.Clear();
                        sb.Append(letter);
                    }
                }
            }
            result.Add(sb.ToString());
            return result;
        }

        private int CompareStrings(string x, string y, bool isLast)
        {
            if (int.TryParse(x, out int xNum1))
            {
                if (int.TryParse(y, out int yNum1))
                    return xNum1.CompareTo(yNum1);

                return -1;
            }

            if (int.TryParse(y, out int number))
                return 1;

            var isXLonger = x.Length > y.Length;
            for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                if (x[i].Equals(y[i])
                    || (Math.Abs(x[i].CompareTo(y[i])) == 32 && !isLast))
                    continue;

                return CompareCharAlphabetic(x[i], y[i]);
            }
            return isXLonger ? 1 : x.Length == y.Length ? 0 : -1;
        }

        private int CompareCharAlphabetic(char x, char y)
        {
            if (char.IsLetter(x) && char.IsLetter(y))
            {
                var comparation = x.CompareTo(y);
                if (char.IsUpper(x))
                    return Math.Sign(comparation);

                if (char.IsLower(x))
                    return char.IsUpper(y) && comparation < 32 ? -1 * Math.Sign(comparation) : Math.Sign(comparation);
                return Math.Sign(comparation);
            }
            return string.Compare(x.ToString(), y.ToString());
        }
    }
}
