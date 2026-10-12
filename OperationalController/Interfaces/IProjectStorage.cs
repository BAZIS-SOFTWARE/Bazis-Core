using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет операции обмена данными проекта с файловой системой.
    /// </summary>
    public interface IProjectStorage
    {
        void LoadSMF(string path);
        void SaveSMF(string path);
        bool CopyFile(string fileName, string oldFolder, string newFolder);
        void ExportMesh(string filePath);
        void ExportGeometry(string filePath);
    }
}
