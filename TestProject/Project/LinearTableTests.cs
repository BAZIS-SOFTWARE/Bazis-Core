#pragma warning disable CS0618 // тесты формата 1.2.0 работают с PropertyItem и FunctionItem до их удаления в 2.0.0
using MaterialDB;
using MaterialDB.FunctionData;
using MaterialDB.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.Tasks.Functions;
using System.Data;

namespace TestProject_test.Project
{
    [TestClass]
    public class LinearTableTests
    {
        private readonly string materialDataFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "MaterialDB", "Data");

        [TestMethod]
        public void MatchesOldInterpolationOnFunctionDatabase()
        {
            var functions = new FunctionDBData("functions.jsf", materialDataFolder);

            foreach (var function in functions.Values.Where(function => function.DataTable.Rows.Count > 0))
            {
                var table = new LinearTable(function);
                var arguments = function.DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
                var values = function.DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[1])).ToArray();
                var first = arguments.Min() - 10;
                var last = arguments.Max() + 10;

                for (var step = 0; step <= 50; step++)
                {
                    var argument = first + (last - first) * step / 50;
                    var expected = InterpolationSearch.InterpolatedValue(arguments, values, argument);
                    var actual = table.Interpolate(argument);

                    Assert.AreEqual(expected, actual, Math.Abs(expected) * 1e-5 + 1e-6, $"{function.Name}, x = {argument}");
                }
            }
        }

        [DataTestMethod]
        [DataRow(-5.0, 10.0)]
        [DataRow(0.0, 10.0)]
        [DataRow(5.0, 15.0)]
        [DataRow(10.0, 20.0)]
        [DataRow(15.0, 40.0)]
        [DataRow(100.0, 60.0)]
        public void InterpolatesAndSaturatesAtEdges(double argument, double expected)
        {
            var table = new LinearTable(CreateFunction(ArgumentKind.Argument, new[] { 0.0, 10.0, 20.0 }, new[] { 10.0, 20.0, 60.0 }));

            Assert.AreEqual(expected, table.Interpolate(argument), 1e-12);
        }

        [TestMethod]
        public void SingleRowIsConstant()
        {
            var table = new LinearTable(CreateFunction(ArgumentKind.Constant, new[] { 5.0 }, new[] { 42.0 }));

            Assert.AreEqual(42.0, table.Interpolate(-100), 1e-12);
            Assert.AreEqual(42.0, table.Interpolate(100), 1e-12);
        }

        [TestMethod]
        public void TableIsSnapshot()
        {
            var function = CreateFunction(ArgumentKind.Argument, new[] { 0.0, 10.0 }, new[] { 0.0, 10.0 });
            var table = new LinearTable(function);

            function.DataTable.Rows[1][1] = 100.0;

            Assert.AreEqual(5.0, table.Interpolate(5), 1e-12);
        }

        [TestMethod]
        public void TableParameterUsesTable()
        {
            var function = CreateFunction(ArgumentKind.Argument, new[] { 0.0, 10.0 }, new[] { 0.0, 100.0 });
            var argument = new global::Project.Tasks.Functions.Parameter("t", ParameterType.Constant, 2.5);
            var parameter = new TableParameter(function, "h", argument);

            Assert.AreEqual(25.0, parameter.GetValue(), 1e-12);
        }

        private FunctionItem CreateFunction(ArgumentKind kind, double[] arguments, double[] values)
        {
            var table = new DataTable();
            table.Columns.Add("X", typeof(double));
            table.Columns.Add("Y", typeof(double));
            for (var row = 0; row < arguments.Length; row++)
                table.Rows.Add(arguments[row], values[row]);

            return new FunctionItem("f", "", "", "", table, kind);
        }
    }
}
