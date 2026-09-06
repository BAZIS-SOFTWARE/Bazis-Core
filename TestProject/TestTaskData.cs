using System.Data;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Project.Interfaces.Tasks;
using Model.GroupsData;
using Model.Interfaces;
using Geometry;
using Project.Tasks;
using Project.TaskParameters;
using Project.Tasks.LocalFrames;
using MaterialDB.MaterialData;
using MaterialDB.FunctionData;
using Project.IO;
using Project.Tasks.Functions;

namespace TestProject
{
    [TestClass]
    public class TestTaskData
    {
        string testDir = @"..\..\..\Examples";
        MaterialDBData materials;
        FunctionDBData functions;

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"{testDir}\materials.jsf"), settingsSerializer);
            functions = JsonConvert.DeserializeObject<FunctionDBData>
    (File.ReadAllText($@"{testDir}\functions.jsf"), settingsSerializer);
        }
        [TestMethod]
        public void RemoveTaskDataByInfoString()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();

            //act
            var proj = loader.Load($@"{testDir}\proj_traj.bpf");

            var dic = proj.TaskData.Find(DataKind.Закрепление).ToArray();

            foreach (var item in dic)
                proj.TaskData.Remove(item);
            //assert
            Assert.AreEqual(proj.TaskData.Count, 3);

        }

        [TestMethod]
        public void RemoveTaskDataByExistState()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load($@"{testDir}\proj_traj.bpf");
            //act

            proj.ModelData.GroupData.RemoveAt(5);
            proj.TaskData.ClearNotExisted(proj.ModelData.GroupData);

            //assert
            Assert.AreEqual(proj.TaskData.Count, 5);

        }

        [TestMethod]
        public void FindTaskDataByType()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var res = proj.TaskData.Find<ICondData>();
            //assert

            Assert.AreEqual(res.Count(), 6);

        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        //[DataRow(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf", DisplayName = "proj_traj_baseLine_not_sorted")]

        //[DataRow(@"c:\projs\Roman\Proba\777.bpf2")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void TestFrameFunctionInTime(string relativeProjPath)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(relativeProjPath);
            var ext = Path.GetExtension(relativeProjPath);

            //act
            var selectedData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (HeatData)x).ToArray();
            foreach (var preFrame in selectedData)
            {
                var stepTime = (preFrame.StopTime - preFrame.StartTime) / 10;
                for (float i = preFrame.StartTime; i < preFrame.StopTime; i+= stepTime)
                {
                    var ff = preFrame.Function as FrameFunction;
                    ff["TIME"].SetValue(i);
  
                    var frame = preFrame.LocalFrame.Frame;

                    if (frame.Centre._x is float.NaN || frame.Centre._y is float.NaN || frame.Centre._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат центра не определена. frame: {preFrame}");
                    if (frame.Dir_X._x is float.NaN || frame.Dir_X._y is float.NaN || frame.Dir_X._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси X не определена. frame: {preFrame}");
                    if (frame.Dir_Y._x is float.NaN || frame.Dir_Y._y is float.NaN || frame.Dir_Y._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Y не определена. frame: {preFrame}");
                    if (frame.Dir_Z._x is float.NaN || frame.Dir_Z._y is float.NaN || frame.Dir_Z._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Z не определена. frame: {preFrame}");
                }

            }
            //var frame = selectedData[0].MovedFrameFunction.CalcFrame(0);
            //assert
        }

        [TestMethod]
        public void TestFrameFunctionSortingTrajectory()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var selectedData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (HeatData)x).ToArray();

            var baseLine = (selectedData[0].LocalFrame as MovedFrame).BaseLine;
            //assert
            var first = baseLine[0];
            var secd = baseLine[1];
            var startLength = new Segment3D(first.CalcCentr(), secd.CalcCentr());
            foreach (var node in baseLine.Skip(1))
            {
                var line = new Segment3D(first.CalcCentr(), node.CalcCentr());
                if(startLength.GetLength() > line.GetLength())
                    throw new Exception("траектория вероятно не отсортирована!");
            }
            //
        }

        [DataTestMethod]
        [DataRow(10.1f,1,0.5f,0)]
        [DataRow(0,0,0,0)]
        [DataRow(0, 0, 0, 0)]
        public void TestFrameFunctionTransformation(float rotation,float x,float y,float z)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();

            heatData.LocalFrame.Rotation_X = rotation;
            heatData.LocalFrame.Shifting = new Point3D(x,y,z);
            //assert
            Assert.AreEqual(heatData.LocalFrame.Rotation_X, rotation);
            Assert.AreEqual(heatData.LocalFrame.Shifting, new Point3D(x,y,z));
        }

        [DataTestMethod]
        [DataRow("1 * * None newGroupЭлемент1D 0 1 Сталь_W_S420 стержни 10")]
        [DataRow("1 * * None newGroupЭлемент1D 0 1 Сталь_Р18 пластины 0.5")]
        [DataRow("1 * * None newGroupЭлемент1D 0 1 Сталь_W_S420 пластины 15")]
        [DataRow("1 * * None newGroupЭлемент3D 0 1 Сталь_Р18")]
        [DataRow("1 * * None newGroupЭлемент3D 3 5 Сталь_20ХМ")]

        //[DataRow("newGroupЭлемент3D Сталь_20ХМ 5 1 *")]
        //[DataRow("newGroupЭлемент3D Сталь_20ХМ  1500 *")]
        //[DataRow("newGroupЭлемент3D Сталь_10 * * *")]
        //[DataRow("newGroupЭлемент3D Сталь_20ХМ 3 5 ISORAPID.277.HM")]

        public void TestMaterialDataCreator(string info)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\MixedMesh\createMat.bpf");
            //act
            var dataKind = DataKind.Материал;
            var material = proj.TaskData.Create(dataKind, info, proj.ModelData.GroupData);
            //assert
            Assert.AreEqual($"{dataKind} : {info}", material.ToString());
        }

        [DataTestMethod]
        [DataRow(DataKind.Нагрев)]
        [DataRow(DataKind.Среда)]
        [DataRow(DataKind.Закрепление)]
        [DataRow(DataKind.Материал)]
        public void TestFindTaskData(DataKind datakind)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();

            //act
            var project = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //assert

            var nodeInfo = project.ModelData.GroupData.Find("Узлы");

            var dataInfo = project.TaskData.Find(datakind);
            if (dataInfo.Count() == 0)
                throw new Exception("Ошибка выгрузки данных");
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        //[DataRow(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf", DisplayName = "proj_traj_baseLine_not_sorted")]

        //[DataRow(@"c:\projs\testProj\TubeToTubeTJoint\proj_v1.bpf")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void CreateTaskData(string relativeProjPath)
        {
            //assign

            var loader = new LoadProjectFromTextFormat();

            //act
            var project = loader.Load(relativeProjPath);
            //assert
            Assert.AreNotEqual(project.TaskData, null);
        }

        [DataTestMethod]
        [DataRow("1 PT,TIME=VAR,POW=Table(Heating;TIME) * None e3dLoad 0 49.99998", DataKind.Нагрев, 11, 230)]
        //[DataRow("0.001 * * None e3dLoad 0 49.99998", DataKind.Нагрев)]
        [DataRow("2.2 PT,TIME=VAR,POW=Table(Heating;TIME) * None e3dLoad 0.0001 49", DataKind.Нагрев, 11, 506)]
        //[DataRow("0 * * X nLoad 0 1500 Жесткое", DataKind.Закрепление)]
        //[DataRow("0 * * Y nLoad 0.255 1500 Жесткое", DataKind.Закрепление)]
        //[DataRow("0.527 * * Z nLoad 0.255 1500 Жесткое", DataKind.Закрепление)]
        //[DataRow("1 * * Z nLoad 0 1500 Сила", DataKind.Нагрузка)]
        //[DataRow("1 * * X nLoad 0 1500 Сила", DataKind.Нагрузка)]
        [DataRow("1 DHEX,CONV=25 * None e2dLoad 0 1500", DataKind.Среда,5, 25)]
        [DataRow("1 TT,TIME=VAR,TEMPS=Table(Термоцикл;TIME) * None nLoad 0 1500 ConstantTemp", DataKind.Среда, 5, 260)]


        public void CheckFunctionTaskDataInTime(string dataString, DataKind dataKind, double time,double correct)
        {
            //assign
            var taskData = new TaskData();
            taskData.Materials = materials;
            taskData.Functions = functions;

            var grData = new GroupData();

            if(dataString.Contains("nLoad"))
                grData.Add(new Group("nLoad", ObjType.Узел));
            else if(dataString.Contains("e2dLoad"))
                grData.Add(new Group("e2dLoad", ObjType.Элемент2D));
            else
                grData.Add(new Group("e3dLoad", ObjType.Элемент3D));

            var condData = taskData.Create(dataKind, dataString, grData);

            //act
            condData.Function["TIME"].SetValue(time);

            var res = condData.Value * condData.Function.CalcValue();

            //assert
            Assert.AreEqual(correct, res,0.1);
            Assert.AreEqual(condData.Kind, dataKind);

        }


        [DataTestMethod]
        [DataRow(new string[] { "тепловая" })]
        [DataRow(new string[] { "механическая" })]
        [DataRow(new string[] { "химическая" })]
        public void GenerateParameterFiles(string[] ar)
        {
            GeneralParameters parameters;
            if (ar[0] == "тепловая")
                parameters = new TermalParameters();
            else if (ar[0] == "механическая")
                parameters = new MechanicalParameters();
            else
                parameters = new ChemicalParameters();

            parameters.SolverSettings = CreateSymmetricSolverSettings();

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            if (parameters is TermalParameters termalParameters)
                termalParameters.MetallurgicalProcesses = new() { "Охлаждение", "Нагрев" };

            var serParams = string.Empty;
            if (ar[0] == "тепловая")
                serParams = JsonConvert.SerializeObject((TermalParameters)parameters, settingsSerializer);
            else if (ar[0] == "механическая")
                serParams = JsonConvert.SerializeObject((MechanicalParameters)parameters, settingsSerializer);
            else
                serParams = JsonConvert.SerializeObject((ChemicalParameters)parameters, settingsSerializer);

            File.WriteAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp", serParams);

            if (ar[0] == "тепловая")
            {
                var desParamsTerm = JsonConvert.DeserializeObject<TermalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
            else if (ar[0] == "механическая")
            {
                var desParamsMech = JsonConvert.DeserializeObject<MechanicalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
            else
            {
                var desParams = JsonConvert.DeserializeObject<ChemicalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
        }

        [TestMethod]
        public void MechanicalParameters_ContainThermalFieldByDefault()
        {
            var parameters = new MechanicalParameters();
            var inputField = parameters.InputFields.Single();

            Assert.AreEqual(PhysicalFieldSet.Thermal, inputField.FieldSet);
            Assert.AreEqual(PhysicalFieldSource.Values, inputField.Source);
            Assert.AreEqual(0, inputField.Values.Count);
            Assert.AreEqual(CalculationMode.Stationary, parameters.CalculationMode);
        }

        [TestMethod]
        public void ChemicalParameters_ContainThermalFieldByDefault()
        {
            var parameters = new ChemicalParameters();
            var inputField = parameters.InputFields.Single();

            Assert.AreEqual(PhysicalFieldSet.Thermal, inputField.FieldSet);
            Assert.AreEqual(PhysicalFieldSource.Values, inputField.Source);
            Assert.AreEqual(0, inputField.Values.Count);
        }

        [TestMethod]
        public void GeneralParameters_WriteInputFields()
        {
            var source = new MechanicalParameters
            {
                SolverSettings = CreateSymmetricSolverSettings(),
                InputFields = new List<PhysicalField>
                {
                    new PhysicalField
                    {
                        FieldSet = PhysicalFieldSet.Thermal,
                        Source = PhysicalFieldSource.ResultFile,
                        FileName = "термическая.db"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(source);
            var parameters = JsonConvert.DeserializeObject<MechanicalParameters>(json);

            Assert.IsNotNull(parameters);
            StringAssert.Contains(json, "\"InputFields\"");
            var inputField = parameters.InputFields.Single();
            Assert.AreEqual(PhysicalFieldSet.Thermal, inputField.FieldSet);
            Assert.AreEqual(PhysicalFieldSource.ResultFile, inputField.Source);
            Assert.AreEqual("термическая.db", inputField.FileName);
            Assert.AreEqual(0, inputField.Values.Count);
        }

        [TestMethod]
        public void GeneralParameters_WriteInitialFieldsByGroup()
        {
            var source = new TermalParameters
            {
                SolverSettings = CreateSymmetricSolverSettings(),
                InitialFields = new List<PhysicalField>
                {
                    new PhysicalField
                    {
                        FieldSet = PhysicalFieldSet.Thermal,
                        Source = PhysicalFieldSource.Values,
                        Values = new Dictionary<string, List<PhysicalFieldValue>>
                        {
                            ["body"] = new List<PhysicalFieldValue>
                            {
                                new PhysicalFieldValue { Quantity = PhysicalQuantity.Temperature, Components = new[] { 20.0 } }
                            },
                            ["weld"] = new List<PhysicalFieldValue>
                            {
                                new PhysicalFieldValue { Quantity = PhysicalQuantity.Temperature, Components = new[] { 500.0 } }
                            }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(source);
            var parameters = JsonConvert.DeserializeObject<TermalParameters>(json);

            Assert.IsNotNull(parameters);
            var field = parameters.InitialFields.Single(field => field.FieldSet == PhysicalFieldSet.Thermal);
            Assert.IsNotNull(field);
            Assert.AreEqual(PhysicalFieldSource.Values, field.Source);
            Assert.AreEqual(20.0, field.Values["body"].Single().Components.Single());
            Assert.AreEqual(500.0, field.Values["weld"].Single().Components.Single());
        }

        [TestMethod]
        public void GeneralParameters_WriteInitialResultField()
        {
            var source = new TermalParameters
            {
                SolverSettings = CreateSymmetricSolverSettings(),
                InitialFields = new List<PhysicalField>
                {
                    new PhysicalField
                    {
                        FieldSet = PhysicalFieldSet.Thermal,
                        Source = PhysicalFieldSource.ResultFile,
                        FileName = "термическая_1_0_3.db"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(source);
            var parameters = JsonConvert.DeserializeObject<TermalParameters>(json);

            Assert.IsNotNull(parameters);
            var field = parameters.InitialFields.Single(field => field.FieldSet == PhysicalFieldSet.Thermal);
            Assert.IsNotNull(field);
            Assert.AreEqual(PhysicalFieldSource.ResultFile, field.Source);
            Assert.AreEqual("термическая_1_0_3.db", field.FileName);
            Assert.AreEqual(0, field.Values.Count);
        }

        [TestMethod]
        public void GeneralParameters_WriteConditionReferencesAsStrings()
        {
            var source = new TermalParameters
            {
                SolverSettings = CreateSymmetricSolverSettings(),
                ActiveConditions = new List<ConditionReference>
                {
                    new ConditionReference(0, DataKind.Материал, "body"),
                    new ConditionReference(3, DataKind.Нагрев, "weld seam")
                }
            };

            var json = JsonConvert.SerializeObject(source);
            var parameters = JsonConvert.DeserializeObject<TermalParameters>(json);

            Assert.IsNotNull(parameters);
            StringAssert.Contains(json, "\"0 Материал body\"");
            StringAssert.Contains(json, "\"3 Нагрев weld seam\"");
            Assert.AreEqual(2, parameters.ActiveConditions.Count);
            Assert.AreEqual(3, parameters.ActiveConditions[1].Index);
            Assert.AreEqual(DataKind.Нагрев, parameters.ActiveConditions[1].Kind);
            Assert.AreEqual("weld seam", parameters.ActiveConditions[1].GroupName);
        }

        [TestMethod]
        public void MetallurgicalProcesses_BelongOnlyToThermalParameters()
        {
            var thermalParameters = new TermalParameters
            {
                MetallurgicalProcesses = new List<string> { "Охлаждение", "Нагрев" }
            };

            var thermalJson = JsonConvert.SerializeObject(thermalParameters);
            var mechanicalJson = JsonConvert.SerializeObject(new MechanicalParameters());

            StringAssert.Contains(thermalJson, "\"MetallurgicalProcesses\"");
            Assert.IsFalse(thermalJson.Contains("TermalProcesses"));
            Assert.IsFalse(mechanicalJson.Contains("MetallurgicalProcesses"));
            Assert.IsNull(typeof(GeneralParameters).GetProperty(nameof(TermalParameters.MetallurgicalProcesses)));
            Assert.IsNull(typeof(TermalParameters).GetProperty("ChemicalFile"));
            Assert.IsNull(typeof(MechanicalParameters).GetProperty("ChemicalFile"));
            Assert.IsNull(typeof(GeneralParameters).GetProperty("InitTemp"));
            Assert.IsNull(typeof(GeneralParameters).GetProperty("InitialConditions"));
            Assert.IsNull(typeof(GeneralParameters).GetProperty("RestartFile"));
            Assert.IsNull(typeof(ChemicalParameters).GetProperty("InitConcentration"));
        }

        [TestMethod]
        public void SolverSettings_WriteConcreteSolverAndMatrixStorage()
        {
            var source = new TermalParameters { SolverSettings = CreateSymmetricSolverSettings() };

            var json = JsonConvert.SerializeObject(source);
            var parameters = JsonConvert.DeserializeObject<TermalParameters>(json);

            Assert.IsNotNull(parameters);
            Assert.AreEqual(LinearSolverKind.ConjugateGradient, parameters.SolverSettings.Solver);
            Assert.AreEqual(MatrixStorageKind.SymmetricCsr, parameters.SolverSettings.MatrixStorage);
            Assert.IsFalse(json.Contains("Auto"));
        }

        [TestMethod]
        public void SolverSettings_AreRequired()
        {
            var exception = Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<TermalParameters>("{}"));

            StringAssert.Contains(exception.Message, "SolverSettings");
        }

        [TestMethod]
        public void SolverAndMatrixStorage_AreRequired()
        {
            var solverException = Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<TermalParameters>("{\"SolverSettings\":{\"MatrixStorage\":\"SymmetricCsr\"}}"));
            var storageException = Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<TermalParameters>("{\"SolverSettings\":{\"Solver\":\"ConjugateGradient\"}}"));

            StringAssert.Contains(solverException.Message, "Solver");
            StringAssert.Contains(storageException.Message, "MatrixStorage");
        }

        private static SolverSettings CreateSymmetricSolverSettings() => new(LinearSolverKind.ConjugateGradient, MatrixStorageKind.SymmetricCsr);
    }
}
