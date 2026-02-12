using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using OperationalController.GmshController;
using Project;
using Project.Tasks;
using System;
using System.IO;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
/// <inheritdoc/>

        public void HideMesh(int elemDim, IGmshController controller)
        {
            if (elemDim == 1)
            {
                foreach (var item in ModelData.ObjectData.E1DCollection.Objects)
                {
                    // определить номер геосущности по item.Number
                    // определить статус геосущности
                    // если она скрыта, то скрыть элемент
                }

            }
            else if (elemDim == 2)
            {
                foreach (var item in ModelData.ObjectData.E2DCollection.Objects)
                {
                    // определить номер геосущности по item.Number
                    // определить статус геосущности
                    // если она скрыта, то скрыть элемент
                }

            }

            else
            {
                foreach (var item in ModelData.ObjectData.E3DCollection.Objects)
                {
                    // определить номер геосущности по item.Number
                    // определить статус геосущности
                    // если она скрыта, то скрыть элемент
                }
            }
        }
    }
}
