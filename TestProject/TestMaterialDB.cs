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
        public void TestMaterialsLoadingFromJsonWithPhaseData(string path, string mat, string cat, string prop, float temp, float res)
        {
            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
            var materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText(path), settingsSerializer);
            var genCat = MaterialDBData.CategoryDic[CategoryEnum.General];
            var strProp = MaterialDBData.PropertyDic[PropertyEnum.Structure];
            var strData = materials[mat][genCat][strProp];

            var phaseData = new PhaseData(strData.DataTable);

            var value = materials[mat][cat][prop].CalcProp(phaseData, temp);

            Assert.AreEqual(value, res);
        }
    }
}
