using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController;

namespace OperationalController_test.OperationalController
{
    [TestClass]
    public class FilePathTests
    {
        [TestMethod]
        public void CopyFile_CopiesBetweenDirectoriesWithSpacesAndUnicode()
        {
            var folder = Path.Combine(Path.GetTempPath(), $"Копирование {Guid.NewGuid():N}");
            var source = Path.Combine(folder, "Исходная папка");
            var destination = Path.Combine(folder, "Новая папка");
            Directory.CreateDirectory(source);
            Directory.CreateDirectory(destination);
            try
            {
                var name = "База материалов.jsf";
                var sourcePath = Path.Combine(source, name);
                var destinationPath = Path.Combine(destination, name);
                File.WriteAllText(sourcePath, "данные");
                var controller = new ProjectController();

                Assert.IsTrue(controller.CopyFile(name, source, destination));
                var copied = File.ReadAllText(destinationPath);
                Assert.AreEqual("данные", copied);
                Assert.IsFalse(controller.CopyFile("missing.jsf", source, destination));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }
    }
}
