using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OperationalController;
using Project.IO;
using System.Data;

namespace Project_test.Project
{
    [TestClass]
    public class ProjectPathTests
    {
        [TestMethod]
        public void Open_LoadsDatabasesWhenProjectPathContainsOnlyFileName()
        {
            var prefix = $"relative-{Guid.NewGuid():N}";
            var projectName = prefix + ".bpf2";
            var materialName = prefix + "-materials.jsf";
            var functionName = prefix + "-functions.jsf";
            try
            {
                File.WriteAllText(materialName, "{}");
                File.WriteAllText(functionName, "{}");
                var lines = new[] { "Проект", $"БазаМатериалов : {materialName}", $"БазаФункций : {functionName}", "#Проект" };
                File.WriteAllLines(projectName, lines);
                var controller = new ProjectController();

                controller.Open(projectName);

                Assert.IsTrue(controller.HasProject);
                Assert.AreEqual(materialName, controller.MaterialsDB.Name);
                Assert.AreEqual(functionName, controller.FunctionsDB.Name);
            }
            finally
            {
                File.Delete(projectName);
                File.Delete(materialName);
                File.Delete(functionName);
            }
        }

        [DataTestMethod]
        [DataRow("bpf")]
        [DataRow("bpf2")]
        public void Open_LoadsAdjacentDatabasesWithSpacesAndUnicode(string extension)
        {
            var folder = Path.Combine(Path.GetTempPath(), $"Рабочий стол {Guid.NewGuid():N}");
            Directory.CreateDirectory(folder);
            try
            {
                var materialName = "База материалов.jsf";
                var functionName = "База функций.jsf";
                var materials = new MaterialDBData();
                var material = new MaterialDBItem("Сталь");
                materials.Add(material.Name, material);
                var functions = new FunctionDBData();
                var table = new DataTable();
                table.Columns.Add("Time", typeof(double));
                table.Columns.Add("Value", typeof(double));
                table.Rows.Add(0d, 42d);
                var function = new Property("Нагрев", "s", "K", "K", table);
                functions.Add(function.Name, function);
                var materialPath = Path.Combine(folder, materialName);
                var functionPath = Path.Combine(folder, functionName);
                var materialJson = JsonConvert.SerializeObject(materials);
                var functionJson = JsonConvert.SerializeObject(functions);
                File.WriteAllText(materialPath, materialJson);
                File.WriteAllText(functionPath, functionJson);
                var projectPath = Path.Combine(folder, $"Проект.{extension}");
                var lines = new[] { "Проект", "Тип : термическая", "Вид : Volume", $"БазаМатериалов :   {materialName}  ", $"БазаФункций :   {functionName}  ", "#Проект" };
                File.WriteAllLines(projectPath, lines);

                var controller = new ProjectController();
                controller.Open(projectPath);

                Assert.IsTrue(controller.HasProject);
                Assert.AreEqual(projectPath, controller.FilePath);
                Assert.AreEqual(materialName, controller.MaterialsDB.Name);
                Assert.AreEqual("Сталь", controller.MaterialsDB["Сталь"].Name);
                Assert.AreEqual(functionName, controller.FunctionsDB.Name);
                Assert.AreEqual(42d, controller.FunctionsDB["Нагрев"][0, 1]);

                var loader = new LoadProjectFromTextFormat();
                var savedPath = Path.Combine(folder, $"Сохранённый проект.{extension}");
                controller.Save(savedPath);
                var reloaded = loader.Load(savedPath);
                Assert.AreEqual("Сталь", reloaded.TaskData.Materials["Сталь"].Name);
                Assert.AreEqual(42d, reloaded.TaskData.Functions["Нагрев"][0, 1]);
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }
    }
}
