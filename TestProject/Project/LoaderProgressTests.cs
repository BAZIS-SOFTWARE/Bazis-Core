using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.MeshObjects;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.IO;
using Project.Tasks.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject_test.Project
{
    /// <summary>
    /// Проверяет загрузку непустых данных через интерфейсы с получателем прогресса и без него.
    /// </summary>
    [TestClass]
    public class LoaderProgressTests
    {
        /// <summary>
        /// Проверяет сетку, ссылки граничного условия и монотонный прогресс чтения проекта.
        /// </summary>
        [DataTestMethod]
        [DataRow("bpf", 0)]
        [DataRow("bpf", 1)]
        [DataRow("bpf", 2)]
        [DataRow("bpf2", 0)]
        [DataRow("bpf2", 1)]
        [DataRow("bpf2", 2)]
        [DataRow("BPF2", 0)]
        [DataRow("BPF2", 1)]
        [DataRow("BPF2", 2)]
        public void ProjectLoader_LoadsMeshAndConditions(string extension, int progressMode)
        {
            var fileName = $"loader-progress-{Guid.NewGuid():N}.{extension}";
            var directory = Path.GetTempPath();
            var path = Path.Combine(directory, fileName);
            var legacyFormat = extension == "bpf";
            var nodesHeader = legacyFormat ? "Узлы" : "Узел";
            var elementsHeader = legacyFormat ? "Элементы1D" : "Элемент1D beams";
            var elementsFooter = legacyFormat ? "#Элементы1D" : "#Элемент1D";
            var elementLine = legacyFormat ? "5 1 2" : "5 0 1 2";
            var groupsHeader = legacyFormat ? "Группы узлов и элементов" : "Группы";
            var lines = new[] { nodesHeader, "1 0 0 0", "2 3 4 5", $"#{nodesHeader}", elementsHeader, elementLine, elementsFooter, groupsHeader, "Группа узлов : heated 1 2", $"#{groupsHeader}", "Проект", "Тип : термическая", "Вид : Volume", "Нагрев : 25 * * None heated 2 10" };
            File.WriteAllLines(path, lines);
            try
            {
                IProjectLoader loader = new LoadProjectFromTextFormat();
                var progress = new RecordedProgress();
                var project = progressMode == 0 ? loader.Load(path) : loader.Load(path, progressMode == 1 ? null : progress);

                Assert.AreEqual(2, project.ModelData.ObjectData.NodesSet.Count);
                var node = project.ModelData.ObjectData.NodesSet[2];
                Assert.AreEqual(3f, node.Position._x);
                Assert.AreEqual(4f, node.Position._y);
                Assert.AreEqual(5f, node.Position._z);
                var element = project.ModelData.ObjectData.Find(ObjType.Элемент1D, 5);
                Assert.IsNotNull(element);
                Assert.AreEqual(TaskKind.термическая, project.TaskData.TaskKind);
                Assert.AreEqual(TaskType.Volume, project.TaskData.TaskType);
                Assert.AreEqual(1, project.TaskData.Count);
                var condition = project.TaskData[0];
                var group = project.ModelData.GroupData.Find("heated");
                Assert.AreSame(group, condition.Group);
                Assert.AreEqual(2, group.Count);
                Assert.AreEqual(DataKind.Нагрев, condition.Kind);
                Assert.AreEqual(25d, condition.Value);
                Assert.AreEqual(2f, condition.StartTime);
                Assert.AreEqual(10f, condition.StopTime);

                if (progressMode == 2)
                {
                    Assert.AreEqual(0, progress.Values[0]);
                    Assert.AreEqual(100, progress.Values[^1]);
                    Assert.IsTrue(progress.Values.All(value => value >= 0 && value <= 100));
                    var orderedValues = progress.Values.OrderBy(value => value).ToArray();
                    var actualValues = progress.Values.ToArray();
                    CollectionAssert.AreEqual(orderedValues, actualValues);
                    Assert.IsTrue(progress.Values.Any(value => value > 0 && value < 100));
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Проверяет чтение граничного условия после начала проекта и сохранение существующей группы.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void TaskLoader_LoadsConditionsForExistingGroups(int progressMode)
        {
            var path = Path.GetTempFileName();
            var lines = new[] { "Нагрев : 999 * * None absent 0 1", "Проект", "Тип : термическая", "Вид : AxiPlain", "Нагрев : 42 * * None heated 3 12" };
            File.WriteAllLines(path, lines);
            try
            {
                var model = new ModelData();
                var position = new Point3D(1, 2, 3);
                var node = new Node(7, position);
                model.ObjectData.NodesSet.Add(node.Number, node);
                model.GroupData.Create("heated", new[] { node });
                var group = model.GroupData.Find("heated");
                ITaskDataLoader loader = new LoadTaskDataTextFormat();
                var progress = new RecordedProgress();
                var task = progressMode == 0 ? loader.Load(path, model.GroupData) : loader.Load(path, model.GroupData, progressMode == 1 ? null : progress);

                Assert.AreEqual(TaskKind.термическая, task.TaskKind);
                Assert.AreEqual(TaskType.AxiPlain, task.TaskType);
                Assert.AreEqual(1, task.Count);
                var condition = task[0];
                Assert.AreEqual(DataKind.Нагрев, condition.Kind);
                Assert.AreEqual(42d, condition.Value);
                Assert.AreEqual(3f, condition.StartTime);
                Assert.AreEqual(12f, condition.StopTime);
                Assert.AreSame(group, condition.Group);
                Assert.AreSame(node, condition.Group[0]);

                if (progressMode == 2)
                {
                    Assert.AreEqual(0, progress.Values[0]);
                    Assert.AreEqual(100, progress.Values[^1]);
                    Assert.IsTrue(progress.Values.All(value => value >= 0 && value <= 100));
                    var orderedValues = progress.Values.OrderBy(value => value).ToArray();
                    var actualValues = progress.Values.ToArray();
                    CollectionAssert.AreEqual(orderedValues, actualValues);
                    Assert.IsTrue(progress.Values.Any(value => value > 0 && value < 100));
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        private sealed class RecordedProgress : IProgress<int>
        {
            public List<int> Values { get; } = new List<int>();

            /// <summary>
            /// Синхронно сохраняет процент, сообщённый загрузчиком.
            /// </summary>
            public void Report(int value)
            {
                Values.Add(value);
            }
        }
    }
}
