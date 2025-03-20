using Model.GroupsData;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelController.MeshObjsCreator
{
    public interface IBeamGroupConnection
    {
        IEnumerable<Beam> Create(Group master, Group slave, double searchRadius, int beamsCount = 0);
    }
}
