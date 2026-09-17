using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции обмена данными проекта с файловой системой.
    /// </summary>
    public interface IProjectStorage
    {
        void Save(string path);
        void Load(string path);
        void LoadSMF(string path);
        void SaveSMF(string path);
        IReadOnlyList<string> Append(string path);
        bool CopyFile(string fileName, string oldFolder, string newFolder);
        void ImportCAD(string projectFile);
        void ImportMesh(string filePath);
        void ExportMesh(string filePath);
        void ExportGeometry(string filePath);
    }
}
