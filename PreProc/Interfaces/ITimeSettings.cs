namespace PreProc.Interfaces
{
    public interface ITimeSettings
    {
        float MaxTimeStep { get; set; }
        float MinTimeStep { get; set; }
        float StartTime { get; set; }
        float StopTime { get; set; }
        float InitTimeStep { get; set; }
    }
}