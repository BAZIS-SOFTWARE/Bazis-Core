namespace OperationalController
{
    /// <summary>
    /// Объединяет специализированные контракты контроллера проекта.
    /// </summary>
    public interface IProjectController : IProjectSession, IProjectEvents, IProjectView, IProjectStorage, IMeshQueries, IMeshCommands, IGeometryQueries, IGeometryCommands, IGroupQueries, IGroupCommands, IConditionQueries, IConditionCommands
    {
    }
}
