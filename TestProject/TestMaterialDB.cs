using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using MaterialDB.MaterialData.MetallurgicalData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TestProject
{
    [TestClass]
    public class TestMaterialDB
    {
        [DataTestMethod]
        [DataRow($@"c:\projs\AdditiveTest\CubeWith2DSupports\materials_v3.jsf", "СтальСВ_20ХМ_AD", "Механические свойства", "Модуль Юнга X", 20.0f, 195122f,
            DisplayName = "СтальСВ_20ХМ_AD Модуль Юнга X 20°C")]
        [DataRow($@"..\..\..\Materials\materials_v7.jsf", "Сталь_20", "Тепловые свойства", "Теплопроводность", 20.0f, 0.0508f,
            DisplayName = "Сталь_20 Теплопроводность 20°C")]
        [DataRow($@"..\..\..\Materials\materials_v7.jsf", "Сталь_20ХМ_MixingPhase", "Тепловые свойства", "Теплопроводность", 200.0f, 0.044f,
            DisplayName = "Сталь_20ХМ_MixingPhase Теплопроводность 20°C")]
        [DataRow($@"..\..\..\Materials\materials_v7.jsf", "Сталь_20ХМ_MixingPhase", "Механические свойства", "Предел текучести", 20.0f, 441.0f,
            DisplayName = "Сталь_20ХМ_MixingPhase Предел текучести 20°C")]
        [DataRow($@"..\..\..\Materials\materials_v7.jsf", "Сталь_20", "Тепловые свойства", "Теплопроводность", 500.0f, 0.042f,
            DisplayName = "Сталь_20 Теплопроводность 500°C")]
        public void TestMaterialsLoadingFromJsonWithPhaseData(string path, string mat, string cat, string prop, float temp, float res)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText(path), settingsSerializer);

            var strData = materials[mat][CategoryEnum.General][PropertyEnum.Structure];

            var phaseData = new PhaseData(strData.DataTable);

            var value = materials[mat][cat][prop].CalcProp(phaseData, temp);

            Assert.AreEqual(value, res);
        }

        [DataTestMethod]
        [DataRow("Сталь_20", "Тепловые свойства", "Теплопроводность", "1,0,0,0,0", 20.0f, 0.0508f)]
        [DataRow("Сталь_20", "Тепловые свойства", "Теплопроводность", "1,0,0,0,0", 500.0f, 0.042f)]
        [DataRow("Сталь_20", "Тепловые свойства", "Теплопроводность", "0,0,0,0,1", 500.0f, 0.021f)]
        public void TestMaterialsLoadingFromJson(string mat, string cat, string prop, string phases, float temp, float res)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"..\..\..\Materials\materials_v7.jsf"), settingsSerializer);

            var phaseValues = phases.Split(',').Select(x => float.Parse(x));

            var phaseData = new PhaseData();
            foreach (var phaseValue in phaseValues)
                phaseData.Add(new Phase<float>() { Name = "", Value = phaseValue });

            var value = materials[mat][cat][prop].CalcProp(phaseData, temp);

            Assert.AreEqual(value, res);
        }

        [DataTestMethod]
        [DataRow("Сталь_20",
    DisplayName = "Сталь_20")]
        [DataRow("Сталь_20ХМ_MixingPhase",
    DisplayName = "Сталь_20ХМ_MixingPhase")]
        [DataRow("Сталь_30ХГСА",
    DisplayName = "Сталь_30ХГСА")]
        [DataRow("Сталь_20ХГСА",
    DisplayName = "Сталь_20ХГСА")]
        [DataRow("Титан_Вт6",
    DisplayName = "Титан_Вт6")]
        [DataRow("Титан_Фиктивный",
    DisplayName = "Титан_Фиктивный")]
        public void TestMaterialsChecking(string mat)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"..\..\..\Materials\materials_v7.jsf"), settingsSerializer);

            Assert.AreEqual(true, materials[mat].CheckThermalProps());
            Assert.AreEqual(true, materials[mat].CheckMechanicalProps());
            Assert.AreEqual(true, materials[mat].CheckPhaseData());

        }

        [DataTestMethod]
        [DataRow(new string[] { "Сталь_20", "Титан_Вт6" }, 9,
DisplayName = "Сталь_20 и Титан_Вт6")]
        [DataRow(new string[] { "Сталь_20" }, 5,
DisplayName = "Сталь_20")]
        [DataRow(new string[] { "Титан_Вт6", "Титан_Фиктивный" }, 5,
DisplayName = "Титан_Вт6 и Титан_Фиктивный")]
        [DataRow(new string[] { "Сталь_20", "Титан_Вт6", "Титан_Фиктивный" }, 10,
DisplayName = "Сталь_20 и Титан_Вт6 и Титан_Фиктивный")]

        public void TestMaterialsGetSummaryPhases(string[] mats, int phaseCount)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"..\..\..\Materials\materials_v7.jsf"), settingsSerializer);

            Assert.AreEqual(phaseCount, materials.GetPhases(mats.ToList()).Count);
        }

        [DataTestMethod]
        [DataRow("Дюриксол.В72", 317.5f, 0.00026f)]
        [DataRow("Дюриксол.В72", 733.0f, 0.001f)]
        [DataRow("ISORAPID.277.HM", 500.0f, 0.004f)]
        public void TestFunctions(string fun, float variable, float res)
        {

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var functions = JsonConvert.DeserializeObject<FunctionDBData>
                 (File.ReadAllText(@"..\..\..\Functions\functions.jsf"), settingsSerializer);

            var value = functions[fun].CalcProp(variable);

            Assert.AreEqual(value, res, 0.000001);
        }

        [DataTestMethod]
        [DataRow("Сталь_20ХМ")]
        public void TestMaterialsCopy(string materialName)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"..\..\..\Materials\materials_v7.jsf"), settingsSerializer);

            var oldMaterial = materials[materialName];
            var newMaterial = materials[materialName].Copy(materialName + "_копия");
            oldMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows[0][1] = 1;
            oldMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows.Add(1, 1);

            Assert.AreNotEqual(oldMaterial.Name, newMaterial.Name);
            Assert.AreNotEqual(oldMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows[0][1], newMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows[0][1]);
            Assert.AreNotEqual(oldMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows.Count, newMaterial["Тепловые свойства"]["Теплопроводность"].DataTable.Rows.Count);
            Assert.AreEqual(oldMaterial.CategoryData.Values.Count, newMaterial.CategoryData.Values.Count);
        }

        [DataTestMethod]
        [DataRow("Коэф.теплоотдачи.воздух")]
        public void TestFunctionsCopy(string functionName)
        {

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var functions = JsonConvert.DeserializeObject<FunctionDBData>
                 (File.ReadAllText(@"..\..\..\Functions\functions.jsf"), settingsSerializer);

            var oldFunction = functions[functionName];
            var newFunction = functions[functionName].Copy(functionName + "_копия");
            oldFunction.DataTable.Rows[0][1] = 20;
            oldFunction.DataTable.Rows.Add(1, 1);

            Assert.AreNotEqual(oldFunction.Name, newFunction.Name);
            Assert.AreNotEqual(oldFunction.DataTable.Rows[0][1], newFunction.DataTable.Rows[0][1]);
            Assert.AreNotEqual(oldFunction.DataTable.Rows.Count, newFunction.DataTable.Rows.Count);
        }
    }
}
