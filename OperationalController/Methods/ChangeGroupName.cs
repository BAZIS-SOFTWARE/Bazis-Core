namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Изменяет имя группы и публикует сведения о переименовании.
        /// </summary>
        public void ChangeGroupName(string oldName, string newName)
        {
            var group = ModelData.GroupData.Find(oldName);
            if (group == null || oldName == newName)
                return;

            group.Name = newName;
            Publish(new GroupRename(group.ObjType, group.Number, oldName, newName));
        }
    }
}
