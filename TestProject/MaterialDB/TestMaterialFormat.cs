#pragma warning disable CS0618 // тесты формата 1.2.0 работают с PropertyItem и FunctionItem до их удаления в 2.0.0
using MaterialDB;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using MaterialDB.MaterialData.MetallurgicalData;
using MaterialDB.MaterialData.Settings;
using MaterialDB.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace TestProject_test.MaterialDB
{
    [TestClass]
    public class TestMaterialFormat
    {
        private readonly string materialDataFolder = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "MaterialDB", "Data");

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
        };

        [TestMethod]
        public void ResolverAssignsArgumentKindToOldMaterials()
        {
            var materials = new MaterialDBData("Materials_v7.jsf", materialDataFolder);

            foreach (var material in materials.Values)
            {
                foreach (var category in material.CategoryData)
                {
                    foreach (var property in category.Value.PropertyData.Values)
                    {
                        Assert.AreNotEqual(ArgumentKind.Undefined, property.ArgumentKind, $"{material.Name} / {category.Key} / {property.Name}");

                        if (category.Key == "Тепловые свойства" || category.Key == "Механические свойства")
                            Assert.AreEqual(ArgumentKind.TemperaturePhase, property.ArgumentKind);
                        if (category.Key == "Металлургия")
                            Assert.AreEqual(ArgumentKind.Process, property.ArgumentKind);
                    }
                }

                Assert.AreEqual(ArgumentKind.Descriptor, material[CategoryEnum.General]["Структура"].ArgumentKind);
            }
        }

        [TestMethod]
        public void ResolverAssignsArgumentKindToOldFunctions()
        {
            var functions = new FunctionDBData("functions.jsf", materialDataFolder);

            foreach (var function in functions.Values)
            {
                var expected = function.DataTable.Rows.Count == 1 ? ArgumentKind.Constant : ArgumentKind.Argument;
                Assert.AreEqual(expected, function.ArgumentKind, function.Name);
            }
        }

        [TestMethod]
        public void ResolverDoesNotOverwriteExplicitKind()
        {
            var material = CreateGas();
            var json = JsonConvert.SerializeObject(new MaterialDBData { { material.Name, material } }, settings);

            var loaded = JsonConvert.DeserializeObject<MaterialDBData>(json, settings)!;

            Assert.AreEqual(ArgumentKind.Argument, loaded[material.Name][CategoryEnum.Thermal]["Теплоемкость"].ArgumentKind);
            Assert.AreEqual(ArgumentKind.Descriptor, loaded[material.Name][CategoryEnum.General]["Модель вязкости"].ArgumentKind);
        }

        [TestMethod]
        public void SaveAndLoadKeepsNewDataWithoutTypeNames()
        {
            var materials = new MaterialDBData("Materials_v7.jsf", materialDataFolder);
            var gas = CreateGas();
            materials.Add(gas.Name, gas);

            var json = JsonConvert.SerializeObject(materials, settings);
            var loaded = JsonConvert.DeserializeObject<MaterialDBData>(json, settings)!;
            var loadedGas = loaded[gas.Name];

            Assert.IsFalse(json.Contains("\"$type\""));
            Assert.IsFalse(json.Contains("Undefined"));
            Assert.IsFalse(json.Contains("PropertyDic"));
            Assert.IsFalse(json.Contains("\"HasMetallurgy\""));
            Assert.AreEqual(MediumKind.Gas, loadedGas.Medium);
            Assert.AreEqual("M", loadedGas[CategoryEnum.General]["Модель плотности"].DataTable.Rows[1]["Параметр"]);
            Assert.AreEqual(0.029, Convert.ToDouble(loadedGas[CategoryEnum.General]["Модель плотности"].DataTable.Rows[1]["Значение"]), 1e-12);
            Assert.AreEqual(0.101325, Convert.ToDouble(loadedGas[CategoryEnum.General]["Опорное давление"][0, 0]), 1e-12);
            Assert.AreEqual(1, (int)loadedGas[CategoryEnum.General]["Модель плотности"][0, 1]);
            Assert.AreEqual(1, (int)loadedGas[CategoryEnum.General]["Модель вязкости"][0, 1]);
            Assert.AreEqual(4, loadedGas[CategoryEnum.General]["Модель вязкости"].DataTable.Rows.Count);
            Assert.AreEqual(ArgumentKind.TemperaturePhase, loaded["Сталь_20"][CategoryEnum.Thermal]["Теплопроводность"].ArgumentKind);
        }

        [TestMethod]
        public void SaveAndLoadFunctionsKeepsArgumentKind()
        {
            var functions = new FunctionDBData("functions.jsf", materialDataFolder);

            var json = JsonConvert.SerializeObject(functions, settings);
            var loaded = JsonConvert.DeserializeObject<FunctionDBData>(json, settings)!;

            Assert.IsFalse(json.Contains("\"$type\""));
            foreach (var function in functions.Values)
                Assert.AreEqual(function.ArgumentKind, loaded[function.Name].ArgumentKind);
        }

        [TestMethod]
        public void MediumDefaultsToSolidForOldSteel()
        {
            var materials = new MaterialDBData("Materials_v7.jsf", materialDataFolder);

            var steel = materials["Сталь_20"];

            Assert.AreEqual(MediumKind.Solid, steel.Medium);
            Assert.IsTrue(steel.HasMetallurgy);
            Assert.IsFalse(steel[CategoryEnum.General].PropertyData.ContainsKey("Агрегатное состояние"));
        }

        [TestMethod]
        public void MaterialWithoutStructureUsesArgumentTables()
        {
            var material = new LiquidMaterialItem("Вода");

            var table = material[CategoryEnum.Thermal]["Теплопроводность"];

            Assert.AreEqual(0, material.GetPhases().Count());
            Assert.IsFalse(material.HasMetallurgy);
            Assert.AreEqual(ArgumentKind.Argument, table.ArgumentKind);
            Assert.AreEqual("Температура", table.DataTable.Columns[0].ColumnName);
            Assert.AreEqual("Значение", table.DataTable.Columns[1].ColumnName);
        }

        [TestMethod]
        public void OrthotropicMaterialGetsDirectionalTables()
        {
            var material = new SolidMaterialItem("Композит", new SolidMaterialSettings { MaterialModel = MaterialModel.Orthotropic });

            var thermal = material[CategoryEnum.Thermal].PropertyData;
            var mechanical = material[CategoryEnum.Mechanical].PropertyData;

            Assert.IsTrue(thermal.ContainsKey("Теплопроводность X") && thermal.ContainsKey("Теплопроводность Z"));
            Assert.IsFalse(thermal.ContainsKey("Теплопроводность"));
            Assert.IsTrue(mechanical.ContainsKey("Модуль Юнга Y") && mechanical.ContainsKey("ТКЛР Z"));
            Assert.IsTrue(material.CheckProps());

            thermal.Remove("Теплопроводность Y");
            Assert.ThrowsException<Exception>(() => material.CheckProps());
        }

        [DataTestMethod]
        [DataRow("Тепловые свойства", "Теплопроводность")]
        [DataRow("Механические свойства", "Модуль Юнга")]
        [DataRow("Механические свойства", "ТКЛР")]
        public void SwitchingToOrthotropicCopiesIndependentPhaseTables(string categoryName, string propertyName)
        {
            var material = new SolidMaterialItem("Композит");
            material.AddPhase("second", 0d);
            var source = material[categoryName][propertyName];
            source.DataTable.Rows.Add(20d, 11d, 12d);
            source.X_unit = "C";
            source.Y_unit = "unit";

            material.SetMaterialModel(MaterialModel.Orthotropic);

            var properties = material[categoryName].PropertyData;
            Assert.IsFalse(properties.ContainsKey(propertyName));
            foreach (var axis in new[] { " X", " Y", " Z" })
            {
                var property = properties[propertyName + axis];
                Assert.AreEqual(ArgumentKind.TemperaturePhase, property.ArgumentKind);
                Assert.AreEqual("second", property.DataTable.Columns[2].ColumnName);
                Assert.AreEqual(11d, property[0, 1]);
                Assert.AreEqual(12d, property[0, 2]);
                Assert.AreEqual("C", property.X_unit);
                Assert.AreEqual("unit", property.Y_unit);
                Assert.AreNotSame(source.DataTable, property.DataTable);
            }

            properties[propertyName + " X"][0, 1] = 21d;
            Assert.AreEqual(11d, properties[propertyName + " Y"][0, 1]);
            Assert.AreEqual(11d, properties[propertyName + " Z"][0, 1]);
            Assert.AreEqual((int)MaterialModel.Orthotropic, (int)material[CategoryEnum.General]["Модель материала"][0, 0]);
            Assert.IsTrue(material.CheckProps());
        }

        [DataTestMethod]
        [DataRow("Тепловые свойства", "Теплопроводность")]
        [DataRow("Механические свойства", "Модуль Юнга")]
        [DataRow("Механические свойства", "ТКЛР")]
        public void SwitchingBackToIsotropicUsesXAxis(string categoryName, string propertyName)
        {
            var material = new SolidMaterialItem("Композит");
            material[categoryName][propertyName].DataTable.Rows.Add(20d, 11d);
            material.SetMaterialModel(MaterialModel.Orthotropic);

            var properties = material[categoryName].PropertyData;
            properties[propertyName + " X"][0, 1] = 21d;
            properties[propertyName + " Y"][0, 1] = 22d;
            properties[propertyName + " Z"][0, 1] = 23d;
            var xTable = properties[propertyName + " X"].DataTable;

            material.SetMaterialModel(MaterialModel.Isotropic);

            Assert.AreEqual(21d, properties[propertyName][0, 1]);
            Assert.AreNotSame(xTable, properties[propertyName].DataTable);
            Assert.IsFalse(properties.ContainsKey(propertyName + " X"));
            Assert.IsFalse(properties.ContainsKey(propertyName + " Y"));
            Assert.IsFalse(properties.ContainsKey(propertyName + " Z"));
            Assert.AreEqual((int)MaterialModel.Isotropic, (int)material[CategoryEnum.General]["Модель материала"][0, 0]);
            Assert.IsTrue(material.CheckProps());
        }

        [TestMethod]
        public void SwitchingMaterialModelWithMissingSourceLeavesMaterialUnchanged()
        {
            var material = new SolidMaterialItem("Композит");
            var thermal = material[CategoryEnum.Thermal].PropertyData;
            var conductivity = thermal["Теплопроводность"];
            material[CategoryEnum.Mechanical].PropertyData.Remove("Модуль Юнга");

            Assert.ThrowsException<InvalidOperationException>(() => material.SetMaterialModel(MaterialModel.Orthotropic));

            Assert.AreSame(conductivity, thermal["Теплопроводность"]);
            Assert.IsFalse(thermal.ContainsKey("Теплопроводность X"));
            Assert.AreEqual((int)MaterialModel.Isotropic, (int)material[CategoryEnum.General]["Модель материала"][0, 0]);
        }

        [TestMethod]
        public void SwitchingMaterialModelPreservesSerializedTables()
        {
            var material = new SolidMaterialItem("Композит");
            material[CategoryEnum.Thermal]["Теплопроводность"].DataTable.Rows.Add(20d, 11d);
            material.SetMaterialModel(MaterialModel.Orthotropic);
            material[CategoryEnum.Thermal]["Теплопроводность X"][0, 1] = 21d;

            var json = JsonConvert.SerializeObject(new MaterialDBData { { material.Name, material } }, settings);
            var loaded = JsonConvert.DeserializeObject<MaterialDBData>(json, settings)![material.Name];

            Assert.IsInstanceOfType(loaded, typeof(SolidMaterialItem));
            Assert.AreEqual(21d, loaded[CategoryEnum.Thermal]["Теплопроводность X"][0, 1]);
            Assert.AreEqual(11d, loaded[CategoryEnum.Thermal]["Теплопроводность Y"][0, 1]);
            Assert.IsTrue(loaded.CheckProps());
        }

        [TestMethod]
        public void GasWithIdealGasDoesNotNeedDensityTable()
        {
            var gas = CreateGas();

            Assert.IsFalse(gas[CategoryEnum.Thermal].PropertyData.ContainsKey("Плотность"));
            Assert.IsTrue(gas.CheckProps());
        }

        [TestMethod]
        public void LiquidGetsViscosityTable()
        {
            var liquid = CreateLiquid();

            Assert.AreEqual(ArgumentKind.Argument, liquid[CategoryEnum.Fluid]["Динамическая вязкость"].ArgumentKind);
            Assert.IsFalse(liquid.CategoryData.ContainsKey("Газодинамические свойства"));
            Assert.IsTrue(liquid.CheckProps());
        }

        [TestMethod]
        public void AddPropsDependsOnMedium()
        {
            var materials = new List<MaterialDBItem> { new SolidMaterialItem("Медь"), new LiquidMaterialItem("Вода"), new GasMaterialItem("Аргон") };

            Assert.IsTrue(materials[0].CategoryData.ContainsKey("Механические свойства"));
            Assert.IsTrue(materials[1].CategoryData.ContainsKey("Гидродинамические свойства"));
            Assert.IsTrue(materials[2].CategoryData.ContainsKey("Газодинамические свойства"));

            foreach (var material in materials)
            {
                Assert.IsTrue(material.CategoryData.ContainsKey("Тепловые свойства"));
                Assert.IsTrue(material.CheckProps());
            }
        }

        [TestMethod]
        public void CheckPropsOfDerivedMaterialChecksThermalProps()
        {
            var liquid = CreateLiquid();
            var gas = CreateGas();
            var solid = CreateCopper();

            liquid[CategoryEnum.Thermal].PropertyData.Remove("Теплоемкость");
            gas[CategoryEnum.Thermal].PropertyData.Remove("Теплоемкость");
            solid[CategoryEnum.Thermal].PropertyData.Remove("Теплоемкость");

            Assert.ThrowsException<Exception>(() => liquid.CheckProps());
            Assert.ThrowsException<Exception>(() => gas.CheckProps());
            Assert.ThrowsException<Exception>(() => solid.CheckProps());
        }

        [TestMethod]
        public void CheckPropsOfDerivedMaterialChecksMediumProps()
        {
            var liquid = CreateLiquid();
            var solid = CreateCopper();

            liquid[CategoryEnum.Fluid].PropertyData.Remove("Динамическая вязкость");
            solid[CategoryEnum.Mechanical].PropertyData.Remove("Модуль Юнга");

            Assert.ThrowsException<Exception>(() => liquid.CheckProps());
            Assert.ThrowsException<Exception>(() => solid.CheckProps());
        }

        [TestMethod]
        public void GasViscosityIsStoredInGasCategory()
        {
            var gas = new GasMaterialItem("Аргон");

            var viscosity = gas[CategoryEnum.Gas]["Динамическая вязкость"];

            Assert.AreEqual("Газодинамические свойства", gas.CategoryDic[CategoryEnum.Gas]);
            Assert.AreEqual(ArgumentKind.TemperaturePressure, viscosity.ArgumentKind);
            Assert.AreEqual(MediumKind.Gas, gas.Medium);
            Assert.IsFalse(gas.CategoryData.ContainsKey("Гидродинамические свойства"));
            Assert.IsTrue(gas.CheckProps());
        }

        [TestMethod]
        public void SolidMaterialGetsDefaultSettings()
        {
            var material = new SolidMaterialItem("Медь");
            var general = material[CategoryEnum.General];
            var crystallization = general["Кристаллизация"].DataTable;
            var conductivity = material[CategoryEnum.Thermal]["Теплопроводность"];

            CollectionAssert.AreEqual(new[] { "phase" }, material.GetPhases().ToList());
            Assert.AreEqual(1d, Convert.ToDouble(general["Структура"].DataTable.Rows[0][1]), 1e-12);
            Assert.AreEqual(ArgumentKind.TemperaturePhase, conductivity.ArgumentKind);
            Assert.AreEqual("phase", conductivity.DataTable.Columns[1].ColumnName);
            Assert.AreEqual(1400d, Convert.ToDouble(crystallization.Rows[0][1]), 1e-12);
            Assert.AreEqual(1500d, Convert.ToDouble(crystallization.Rows[1][1]), 1e-12);
            Assert.AreEqual(2, (int)general["Модель упрочнения"][0, 0]);
            Assert.AreEqual(0, (int)general["Модель твердости"][0, 0]);
            Assert.IsTrue(material.CategoryData.ContainsKey("Металлургия"));
            Assert.AreEqual(0, material[CategoryEnum.Metallurgical].PropertyData.Count);
            Assert.IsTrue(material.CheckPhaseData());
        }

        [TestMethod]
        public void SolidConstructorRejectsInconsistentSettings()
        {
            var noPhases = new SolidMaterialSettings { Phases = new PhaseData() };
            var wrongInterval = new SolidMaterialSettings { SolidusTemperature = 1500, LiquidusTemperature = 1400 };
            var hardnessWithoutIndices = new SolidMaterialSettings { HardnessModel = 1 };

            Assert.ThrowsException<ArgumentException>(() => new SolidMaterialItem("Сталь", noPhases));
            Assert.ThrowsException<ArgumentException>(() => new SolidMaterialItem("Сталь", wrongInterval));
            Assert.ThrowsException<ArgumentException>(() => new SolidMaterialItem("Сталь", hardnessWithoutIndices));
        }

        [TestMethod]
        public void AddPhaseAddsZeroColumnToPhaseTables()
        {
            var steel = CreateSteel();
            var conductivity = steel[CategoryEnum.Thermal]["Теплопроводность"].DataTable;
            var yield = steel[CategoryEnum.Mechanical]["Предел текучести"].DataTable;

            steel.AddPhase("M", 0);

            CollectionAssert.AreEqual(new[] { "F", "A", "M" }, steel.GetPhases().ToList());
            Assert.AreEqual("M", conductivity.Columns[3].ColumnName);
            Assert.IsTrue(conductivity.Rows.Count > 0);
            foreach (DataRow row in conductivity.Rows)
                Assert.AreEqual(0d, Convert.ToDouble(row["M"]), 1e-12);
            Assert.IsTrue(yield.Columns.Contains("M"));
            Assert.ThrowsException<ArgumentException>(() => steel.AddPhase("M", 0));
        }

        [TestMethod]
        public void RemovePhaseRemovesRelatedData()
        {
            var settings = CreateSteelSettings();
            settings.Phases.Add(new Phase { Name = "M", Value = 0 });
            settings.HardnessModel = 1;
            settings.HardnessIndices = new HardnessPhaseIndices(0, -1, -1, 2, 1);
            var steel = new SolidMaterialItem("Сталь", settings);
            FillTables(steel);
            AddFilledTable(steel, CategoryEnum.Metallurgical, "Нагрев F-A", ArgumentKind.Process);
            AddFilledTable(steel, CategoryEnum.Metallurgical, "Охлаждение A-M", ArgumentKind.Process);

            steel.RemovePhase("F");

            var hardness = steel[CategoryEnum.General]["Модель твердости"].DataTable.Rows[0];
            var processes = steel[CategoryEnum.Metallurgical].PropertyData;
            CollectionAssert.AreEqual(new[] { "A", "M" }, steel.GetPhases().ToList());
            Assert.IsFalse(steel[CategoryEnum.Thermal]["Теплопроводность"].DataTable.Columns.Contains("F"));
            Assert.IsFalse(steel[CategoryEnum.Mechanical]["ТКЛР"].DataTable.Columns.Contains("F"));
            Assert.IsFalse(processes.ContainsKey("Нагрев F-A"));
            Assert.IsTrue(processes.ContainsKey("Охлаждение A-M"));
            Assert.AreEqual(1, Convert.ToInt32(hardness["Модель твердости"]));
            Assert.AreEqual(-1, Convert.ToInt32(hardness["FerriteIndex"]));
            Assert.AreEqual(1, Convert.ToInt32(hardness["MartensiteIndex"]));
            Assert.AreEqual(0, Convert.ToInt32(hardness["AusteniteIndex"]));
        }

        [TestMethod]
        public void RemovePhaseRejectsUnknownAndLastPhase()
        {
            var material = new SolidMaterialItem("Медь");

            Assert.ThrowsException<ArgumentException>(() => material.RemovePhase("F"));
            Assert.ThrowsException<InvalidOperationException>(() => material.RemovePhase("phase"));
        }

        [TestMethod]
        public void ConstructorWritesGasSettings()
        {
            var gas = CreateGas();
            var general = gas[CategoryEnum.General].PropertyData;

            Assert.IsFalse(gas[CategoryEnum.Thermal].PropertyData.ContainsKey("Плотность"));
            Assert.IsTrue(gas.CategoryData.ContainsKey("Газодинамические свойства"));
            Assert.IsFalse(gas[CategoryEnum.Gas].PropertyData.ContainsKey("Динамическая вязкость"));
            Assert.IsFalse(general.ContainsKey("Молярная масса"));
            Assert.AreEqual(2, general["Модель плотности"].DataTable.Rows.Count);
            Assert.AreEqual(0.029, Convert.ToDouble(general["Модель плотности"].DataTable.Rows[1]["Значение"]), 1e-12);
            Assert.AreEqual(0.101325, Convert.ToDouble(general["Опорное давление"][0, 0]), 1e-12);
            var viscosityModel = general["Модель вязкости"].DataTable;
            Assert.IsFalse(general.ContainsKey("Коэффициенты Сазерленда"));
            Assert.AreEqual(4, viscosityModel.Rows.Count);
            Assert.AreEqual("Модель", viscosityModel.Rows[0]["Параметр"]);
            Assert.AreEqual(1d, Convert.ToDouble(viscosityModel.Rows[0]["Значение"]), 1e-12);
            Assert.AreEqual("μ0", viscosityModel.Rows[1]["Параметр"]);
            Assert.AreEqual(110.4, Convert.ToDouble(viscosityModel.Rows[3]["Значение"]), 1e-12);
            Assert.IsTrue(gas.CheckProps());
        }

        [TestMethod]
        public void GasSwitchesToTableModels()
        {
            var gas = CreateGas();
            var general = gas[CategoryEnum.General].PropertyData;

            gas.UseTableDensity();
            gas.UseTableViscosity();

            Assert.AreEqual(0, (int)general["Модель плотности"][0, 1]);
            Assert.AreEqual(1, general["Модель плотности"].DataTable.Rows.Count);
            Assert.AreEqual(0, (int)general["Модель вязкости"][0, 1]);
            Assert.AreEqual(1, general["Модель вязкости"].DataTable.Rows.Count);
            Assert.IsFalse(general.ContainsKey("Молярная масса"));
            Assert.IsFalse(general.ContainsKey("Коэффициенты Сазерленда"));
            Assert.IsTrue(general.ContainsKey("Опорное давление"));
            Assert.IsTrue(gas[CategoryEnum.Thermal].PropertyData.ContainsKey("Плотность"));
            Assert.AreEqual(ArgumentKind.TemperaturePressure, gas[CategoryEnum.Gas]["Динамическая вязкость"].ArgumentKind);
            Assert.IsTrue(gas.CheckProps());
        }

        [TestMethod]
        public void GasSwitchesToIdealGasAndSutherland()
        {
            var gas = new GasMaterialItem("Аргон");
            var general = gas[CategoryEnum.General].PropertyData;
            var densityModel = general["Модель плотности"].DataTable;
            var viscosityModel = general["Модель вязкости"].DataTable;
            var densityTable = gas[CategoryEnum.Thermal]["Плотность"].DataTable;
            var viscosityTable = gas[CategoryEnum.Gas]["Динамическая вязкость"].DataTable;

            gas.UseIdealGasDensity(0.03);
            gas.UseIdealGasDensity(0.04);
            gas.UseSutherlandViscosity(new SutherlandCoefficients(2.1e-11, 273.15, 144));
            gas.UseSutherlandViscosity(new SutherlandCoefficients(2.2e-11, 273.15, 144));

            Assert.AreSame(densityModel, general["Модель плотности"].DataTable);
            Assert.AreEqual(1, (int)general["Модель плотности"][0, 1]);
            Assert.AreEqual(2, densityModel.Rows.Count);
            Assert.AreEqual(0.04, Convert.ToDouble(densityModel.Rows[1]["Значение"]), 1e-12);
            Assert.IsFalse(general.ContainsKey("Молярная масса"));
            Assert.AreSame(viscosityModel, general["Модель вязкости"].DataTable);
            Assert.AreEqual(1, (int)general["Модель вязкости"][0, 1]);
            Assert.AreEqual(4, viscosityModel.Rows.Count);
            Assert.AreEqual(2.2e-11, Convert.ToDouble(viscosityModel.Rows[1]["Значение"]), 1e-20);
            Assert.AreSame(densityTable, gas[CategoryEnum.Thermal]["Плотность"].DataTable);
            Assert.AreSame(viscosityTable, gas[CategoryEnum.Gas]["Динамическая вязкость"].DataTable);
            Assert.IsTrue(gas.CheckProps());

            gas.UseTableDensity();
            gas.UseTableViscosity();

            Assert.AreSame(densityModel, general["Модель плотности"].DataTable);
            Assert.AreEqual(1, densityModel.Rows.Count);
            Assert.AreEqual(0, (int)general["Модель плотности"][0, 1]);
            Assert.AreSame(viscosityModel, general["Модель вязкости"].DataTable);
            Assert.AreEqual(1, viscosityModel.Rows.Count);
            Assert.AreEqual(0, (int)general["Модель вязкости"][0, 1]);
            Assert.AreSame(densityTable, gas[CategoryEnum.Thermal]["Плотность"].DataTable);
            Assert.AreSame(viscosityTable, gas[CategoryEnum.Gas]["Динамическая вязкость"].DataTable);
        }

        [TestMethod]
        public void ValidatorChecksSutherlandRowsInViscosityModel()
        {
            var validator = new MaterialValidator(new PressureColumnConvention());
            var missing = CreateGas();
            var negative = CreateGas();

            missing[CategoryEnum.General]["Модель вязкости"].DataTable.Rows.RemoveAt(3);
            negative[CategoryEnum.General]["Модель вязкости"].DataTable.Rows[1]["Значение"] = -1d;

            Assert.IsTrue(validator.Validate(missing).Any(issue => issue.Message.StartsWith("V7:") && issue.Message.Contains("«S»")));
            Assert.IsTrue(validator.Validate(negative).Any(issue => issue.Message.StartsWith("V8:")));
            Assert.ThrowsException<Exception>(() => missing.CheckProps());
            Assert.IsFalse(validator.Validate(CreateGas()).Any(issue => issue.Message.Contains("Сазерленд")));
        }

        [TestMethod]
        public void ValidatorChecksMolarMassRowInDensityModel()
        {
            var validator = new MaterialValidator(new PressureColumnConvention());
            var missing = CreateGas();
            var negative = CreateGas();

            missing[CategoryEnum.General]["Модель плотности"].DataTable.Rows.RemoveAt(1);
            negative[CategoryEnum.General]["Модель плотности"].DataTable.Rows[1]["Значение"] = -1d;

            Assert.IsTrue(validator.Validate(missing).Any(issue => issue.Message.StartsWith("V7:") && issue.Message.Contains("«M»")));
            Assert.IsTrue(validator.Validate(negative).Any(issue => issue.Message.StartsWith("V8:") && issue.Message.Contains("«M»")));
            Assert.ThrowsException<Exception>(() => missing.CheckProps());
            Assert.IsFalse(validator.Validate(CreateGas()).Any(issue => issue.Message.Contains("«M»")));
        }

        [TestMethod]
        public void ReferencePressureDefaultsToOne()
        {
            var liquid = new LiquidMaterialItem("Вода");
            var gas = new GasMaterialItem("Аргон");

            Assert.AreEqual(1d, Convert.ToDouble(liquid[CategoryEnum.General]["Опорное давление"][0, 0]), 1e-12);
            Assert.AreEqual(1d, Convert.ToDouble(gas[CategoryEnum.General]["Опорное давление"][0, 0]), 1e-12);
        }

        [TestMethod]
        public void GasConstructorRequiresModelData()
        {
            var idealGas = new GasMaterialSettings { DensityModel = DensityModel.IdealGas };
            var sutherland = new GasMaterialSettings { ViscosityModel = ViscosityModel.Sutherland };

            Assert.ThrowsException<ArgumentException>(() => new GasMaterialItem("Газ", idealGas));
            Assert.ThrowsException<ArgumentException>(() => new GasMaterialItem("Газ", sutherland));
        }

        [TestMethod]
        public void CopyKeepsMaterialComposition()
        {
            var steel = CreateSteel();

            var copy = steel.Copy("Сталь_копия");

            Assert.IsInstanceOfType(copy, typeof(SolidMaterialItem));
            Assert.AreEqual("Сталь_копия", copy.Name);
            Assert.AreEqual(steel.CategoryData.Count, copy.CategoryData.Count);
            Assert.AreEqual(steel[CategoryEnum.General].PropertyData.Count, copy[CategoryEnum.General].PropertyData.Count);
            Assert.AreEqual(steel.GetPhases().Count(), copy.GetPhases().Count());
            Assert.IsTrue(copy.CheckProps());
        }

        [TestMethod]
        public void MaterialClassFollowsMedium()
        {
            Assert.AreEqual(MediumKind.Solid, new SolidMaterialItem("s").Medium);
            Assert.AreEqual(MediumKind.Liquid, new LiquidMaterialItem("l").Medium);
            Assert.AreEqual(MediumKind.Gas, new GasMaterialItem("g").Medium);
        }

        [TestMethod]
        public void GeneralPropsWriteMediumOnlyForFluids()
        {
            var steel = CreateSteel();
            var gas = CreateGas();
            var liquid = CreateLiquid();

            Assert.IsFalse(steel[CategoryEnum.General].PropertyData.ContainsKey("Агрегатное состояние"));
            Assert.AreEqual(2, (int)gas[CategoryEnum.General]["Агрегатное состояние"][0, 0]);
            Assert.AreEqual(1, (int)liquid[CategoryEnum.General]["Агрегатное состояние"][0, 0]);
        }

        [TestMethod]
        public void LoadingChoosesClassByMedium()
        {
            var materials = new MaterialDBData("Materials_v7.jsf", materialDataFolder);
            materials.Add("Воздух", CreateGas());
            materials.Add("Вода", CreateLiquid());

            var json = JsonConvert.SerializeObject(materials, settings);
            var loaded = JsonConvert.DeserializeObject<MaterialDBData>(json, settings)!;

            Assert.IsFalse(json.Contains("\"$type\""));
            Assert.IsInstanceOfType(loaded["Воздух"], typeof(GasMaterialItem));
            Assert.IsInstanceOfType(loaded["Вода"], typeof(LiquidMaterialItem));
            foreach (var material in loaded.Values.Where(item => item.Name != "Воздух" && item.Name != "Вода"))
                Assert.IsInstanceOfType(material, typeof(SolidMaterialItem), material.Name);
        }

        [TestMethod]
        public void CopyKeepsMaterialClass()
        {
            Assert.IsInstanceOfType(CreateGas().Copy("копия"), typeof(GasMaterialItem));
            Assert.IsInstanceOfType(CreateLiquid().Copy("копия"), typeof(LiquidMaterialItem));
            Assert.IsInstanceOfType(CreateSteel().Copy("копия"), typeof(SolidMaterialItem));
        }

        [TestMethod]
        public void UnknownMediumInFileIsRejected()
        {
            var json = "{\"Х\":{\"Name\":\"Х\",\"CategoryData\":{\"Общие сведения\":{\"Name\":\"Общие сведения\",\"PropertyData\":{\"Агрегатное состояние\":{\"Name\":\"Агрегатное состояние\",\"DataTable\":[{\"Агрегатное состояние\":7}]}}}}}}";

            Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<MaterialDBData>(json, settings));
        }

        [TestMethod]
        public void CategoryIndexersFindRussianNames()
        {
            var material = CreateSteel();

            Assert.AreEqual("Теплоемкость", material[CategoryEnum.Thermal].PropertyDic[PropertyEnum.Capacity]);
            Assert.AreSame(material[CategoryEnum.Thermal], material.CategoryData[PropertyEnum.Capacity]);
            Assert.AreSame(material[CategoryEnum.Mechanical], material.CategoryData[PropertyEnum.YieldStrength]);
            Assert.AreSame(material[CategoryEnum.General], material.CategoryData[PropertyEnum.Structure]);
        }

        [TestMethod]
        public void PressureColumnConventionUsesInvariantCulture()
        {
            var culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            try
            {
                var convention = new PressureColumnConvention();
                var table = new DataTable();
                table.Columns.Add("Температура", typeof(double));
                table.Columns.Add(convention.Format(0.101325), typeof(double));
                table.Columns.Add("P_0.5", typeof(double));

                Assert.AreEqual("P_0.101325", convention.Format(0.101325));
                Assert.IsTrue(convention.TryParse("P_0.5", out var pressure));
                Assert.AreEqual(0.5, pressure);
                Assert.IsFalse(convention.TryParse("P_abc", out _));
                Assert.IsFalse(convention.TryParse("Температура", out _));
                CollectionAssert.AreEqual(new[] { 0.101325, 0.5 }, convention.GetPressures(table).ToArray());
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }

        [TestMethod]
        public void ValidMaterialsHaveNoErrors()
        {
            var validator = new MaterialValidator(new PressureColumnConvention());

            foreach (var material in new MaterialDBItem[] { CreateSteel(), CreateGas(), CreateLiquid() })
            {
                var errors = validator.Validate(material).Where(issue => issue.Severity == IssueSeverity.Error).ToList();
                Assert.AreEqual(0, errors.Count, $"{material.Name}: {string.Join("; ", errors)}");
            }
        }

        [TestMethod]
        public void ValidFunctionsHaveNoErrors()
        {
            var functions = new FunctionDBData("functions.jsf", materialDataFolder);
            var validator = new MaterialValidator(new PressureColumnConvention());

            var errors = validator.Validate(functions).Where(issue => issue.Severity == IssueSeverity.Error).ToList();
            var emptyFunctions = functions.Values.Where(function => function.DataTable.Rows.Count == 0).Select(function => function.Name).ToList();

            // В functions.jsf есть пустые функции («Соль(0.001500)», «Heating»): их таблицы 0×0 — это правило V2.
            Assert.IsTrue(errors.All(issue => emptyFunctions.Contains(issue.Item) && issue.Message.StartsWith("V2:")), string.Join("; ", errors));
        }

        [DataTestMethod]
        [DataRow("V1")]
        [DataRow("V2")]
        [DataRow("V3")]
        [DataRow("V4")]
        [DataRow("V5")]
        [DataRow("V6")]
        [DataRow("V7")]
        [DataRow("V8")]
        [DataRow("V9")]
        [DataRow("V10")]
        [DataRow("V11")]
        [DataRow("V12")]
        [DataRow("V13")]
        [DataRow("V14")]
        [DataRow("V15")]
        public void ValidatorReportsRule(string rule)
        {
            var material = CreateBrokenMaterial(rule);
            var validator = new MaterialValidator(new PressureColumnConvention());

            var issues = validator.Validate(material).ToList();

            Assert.IsTrue(issues.Any(issue => issue.Message.StartsWith(rule + ":")), string.Join("; ", issues));
        }

        [TestMethod]
        public void ValidatorReportsUndefinedFunction()
        {
            var functions = new FunctionDBData();
            var table = new DataTable();
            table.Columns.Add("X", typeof(double));
            table.Columns.Add("Y", typeof(double));
            table.Rows.Add(0d, 1d);
            functions.Add("f", new FunctionItem("f", "", "", "", table));

            var issues = new MaterialValidator(new PressureColumnConvention()).Validate(functions).ToList();

            Assert.IsTrue(issues.Any(issue => issue.Message.StartsWith("V1:")));
        }

        [TestMethod]
        public void HardnessIndicesAreStoredInMaterial()
        {
            var settings = CreateSteelSettings();
            settings.HardnessModel = 1;
            settings.HardnessIndices = new HardnessPhaseIndices(0, -1, -1, -1, 1);
            var material = new SolidMaterialItem("Сталь", settings);
            FillTables(material);

            var table = material[CategoryEnum.General]["Модель твердости"];
            var issues = new MaterialValidator(new PressureColumnConvention()).Validate(material);

            Assert.IsFalse(material[CategoryEnum.General].PropertyData.ContainsKey("Индексы фаз модели твердости"));
            Assert.AreEqual(ArgumentKind.Descriptor, table.ArgumentKind);
            Assert.AreEqual(1, table.DataTable.Rows.Count);
            Assert.AreEqual(1, Convert.ToInt32(table.DataTable.Rows[0]["Модель твердости"]));
            Assert.AreEqual(0, Convert.ToInt32(table.DataTable.Rows[0]["FerriteIndex"]));
            Assert.AreEqual(1, Convert.ToInt32(table.DataTable.Rows[0]["AusteniteIndex"]));
            Assert.AreEqual(1, (int)table[0, 0]);
            Assert.IsFalse(issues.Any(issue => issue.Severity == IssueSeverity.Error));
        }

        private MaterialDBItem CreateBrokenMaterial(string rule)
        {
            var steel = CreateSteel();
            switch (rule)
            {
                case "V1":
                    steel[CategoryEnum.Thermal]["Теплоемкость"].ArgumentKind = ArgumentKind.Undefined;
                    return steel;
                case "V2":
                    steel[CategoryEnum.Thermal]["Теплоемкость"].DataTable.Rows.Clear();
                    return steel;
                case "V3":
                    steel[CategoryEnum.Thermal]["Теплоемкость"].DataTable.Rows[1][0] = 10d;
                    return steel;
                case "V4":
                    steel[CategoryEnum.Thermal]["Теплоемкость"].DataTable.Rows[1][1] = double.NaN;
                    return steel;
                case "V5":
                    steel[CategoryEnum.Thermal]["Теплоемкость"].DataTable.Columns[1].ColumnName = "Мартенсит";
                    return steel;
                case "V6":
                    var water = CreateLiquid();
                    water[CategoryEnum.Thermal]["Теплоемкость"].ArgumentKind = ArgumentKind.TemperaturePhase;
                    return water;
                case "V7":
                    steel[CategoryEnum.Mechanical].PropertyData.Remove("Предел текучести");
                    return steel;
                case "V8":
                    var liquid = CreateLiquid();
                    AddGeneralInt(liquid, "Модель плотности", (int)DensityModel.IdealGas);
                    return liquid;
                case "V9":
                    var gas = CreateGas();
                    var structure = new DataTable();
                    structure.Columns.Add("Фаза", typeof(string));
                    structure.Columns.Add("Масс.доли", typeof(double));
                    structure.Rows.Add("A", 1d);
                    gas[CategoryEnum.General].PropertyData.Add("Структура", new PropertyItem("Структура", "", "", "", structure, ArgumentKind.Descriptor));
                    return gas;
                case "V10":
                    steel[CategoryEnum.General]["Структура"].DataTable.Rows[0][1] = 0.9;
                    return steel;
                case "V11":
                    var idealGas = CreateGas();
                    AddFilledTable(idealGas, CategoryEnum.Thermal, "Плотность", ArgumentKind.Argument);
                    return idealGas;
                case "V12":
                    var sutherlandGas = CreateGas();
                    AddFilledTable(sutherlandGas, CategoryEnum.Gas, "Динамическая вязкость", ArgumentKind.Argument);
                    return sutherlandGas;
                case "V13":
                    steel[CategoryEnum.General]["Модель материала"].DataTable.Rows[0][0] = (int)MaterialModel.Orthotropic;
                    return steel;
                case "V15":
                    var misplaced = CreateGas();
                    misplaced.CategoryData.Add("Гидродинамические свойства", new Category { Name = "Гидродинамические свойства" });
                    AddFilledTable(misplaced, CategoryEnum.Fluid, "Динамическая вязкость", ArgumentKind.Argument);
                    return misplaced;
                case "V14":
                    var hardnessSettings = CreateSteelSettings();
                    hardnessSettings.HardnessModel = 1;
                    hardnessSettings.HardnessIndices = new HardnessPhaseIndices(0, 0, -1, -1, 7);
                    var hardnessSteel = new SolidMaterialItem("Сталь", hardnessSettings);
                    FillTables(hardnessSteel);
                    return hardnessSteel;
            }

            throw new ArgumentException(rule);
        }

        private SolidMaterialItem CreateSteel()
        {
            var material = new SolidMaterialItem("Сталь", CreateSteelSettings());
            FillTables(material);
            return material;
        }

        private SolidMaterialSettings CreateSteelSettings()
        {
            var phases = new PhaseData(new List<Phase>
            {
                new Phase { Name = "F", Value = 0.5f },
                new Phase { Name = "A", Value = 0.5f }
            });
            return new SolidMaterialSettings { Phases = phases };
        }

        private SolidMaterialItem CreateCopper()
        {
            var material = new SolidMaterialItem("Медь");
            FillTables(material);
            return material;
        }

        private GasMaterialItem CreateGas()
        {
            var gasSettings = new GasMaterialSettings
            {
                DensityModel = DensityModel.IdealGas,
                ViscosityModel = ViscosityModel.Sutherland,
                MolarMass = 0.029,
                ReferencePressure = 0.101325,
                Sutherland = new SutherlandCoefficients(1.716e-11, 273.15, 110.4)
            };
            var material = new GasMaterialItem("Воздух", gasSettings);
            FillTables(material);
            return material;
        }

        private LiquidMaterialItem CreateLiquid()
        {
            var material = new LiquidMaterialItem("Вода");
            FillTables(material);
            return material;
        }

        private void AddGeneralInt(MaterialDBItem material, string name, int value)
        {
            var table = new DataTable();
            table.Columns.Add(name, typeof(int));
            table.Rows.Add(value);
            material[CategoryEnum.General].PropertyData.Add(name, new PropertyItem(name, "", "", "", table, ArgumentKind.Constant));
        }

        private void AddFilledTable(MaterialDBItem material, CategoryEnum category, string name, ArgumentKind kind)
        {
            var categoryName = material.CategoryDic[category];
            if (!material.CategoryData.ContainsKey(categoryName))
                material.CategoryData.Add(categoryName, new Category { Name = categoryName });

            var table = new DataTable();
            table.Columns.Add("Температура", typeof(double));
            table.Columns.Add("Значение", typeof(double));
            table.Rows.Add(20d, 1d);
            material[category].PropertyData.Add(name, new PropertyItem(name, "", "", "", table, kind));
        }

        private void FillTables(MaterialDBItem material)
        {
            foreach (var category in material.CategoryData)
            {
                if (category.Key == "Общие сведения" || category.Key == "Металлургия")
                    continue;

                foreach (var property in category.Value.PropertyData.Values)
                {
                    var table = property.DataTable;
                    foreach (var temperature in new[] { 20d, 500d })
                    {
                        var row = table.NewRow();
                        row[0] = temperature;
                        for (var column = 1; column < table.Columns.Count; column++)
                            row[column] = 1d + temperature / 1000;
                        table.Rows.Add(row);
                    }
                }
            }
        }
    }
}
