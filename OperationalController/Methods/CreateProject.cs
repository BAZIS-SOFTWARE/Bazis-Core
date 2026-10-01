using Model.IO;
using Model.IO.STL;
using Project;
using Project.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        public void CreateProject(string projectFile)
        {
            Create(projectFile);
        }
    }
}
