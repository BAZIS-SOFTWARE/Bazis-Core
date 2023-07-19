using Model.GmshWrapper;
using System;

namespace Model.GmshObject
{
    public class ModelObject : IModelApi, IDisposable
    {
        protected long Id;

        public ModelObject()
        {
            Id = GmshWrapperConnection.GetId();
            GmshWrapperModel.ModelAdd(Id.ToString());
        }
        public ModelObject(string path)
        {
            Id = GmshWrapperConnection.GetId();
            GmshWrapperModel.ModelAdd(Id.ToString());
            Load(path);
        }

        public void SetCurrent() => GmshWrapperModel.ModelSetCurrent(Id.ToString());

        public void Load(string filePath)
        {
            SetCurrent();
            GmshWrapperIO.Merge(filePath);
            GmshWrapperIO.ModelSetFileName(filePath);
        }
        /// <summary>
        /// Удалить сущность по щелчку мыши на неё
        /// </summary>
        /// <param name="entityId">[In]id</param>
        public void DeleteEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        virtual public void Dispose() => GmshWrapperModel.ModelRemoveByName(Id.ToString());
    }
}
