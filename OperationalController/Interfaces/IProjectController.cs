namespace OperationalController
{
    /// <summary>
    /// Объединяет специализированные контракты контроллера проекта.
    /// </summary>
    public interface IProjectController : IProjectSessionQueries, IProjectStorage, IMeshQueries, IMeshCommands, IGeometryQueries, IGeometryCommands, IGroupQueries, IGroupCommands, IConditionQueries, IConditionCommands
    {
    }
}
