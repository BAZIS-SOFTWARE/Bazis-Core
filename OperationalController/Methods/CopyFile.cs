using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        public bool CopyFile(string fileName, string oldFolder, string newFolder)
        {
            var oldfilePath = $@"{oldFolder}\{fileName}";

            if (File.Exists(oldfilePath))
            {
                var newfilePath = $@"{newFolder}\{fileName}";

                File.Create(newfilePath).Close();
                File.Copy(oldfilePath, newfilePath, true);
                return true;
            }
            else return false;
        }
    }
}
