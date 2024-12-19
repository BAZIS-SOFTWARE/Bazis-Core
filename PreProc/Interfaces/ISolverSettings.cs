namespace Tasks.Interfaces
{
    public interface ISolverSettings
    {
        int MaxIter { get; set; }
        string Solver { get; set; }

        float MaxRelaxation { get; set; }
        float Relaxation { get; set; }

        float Precision { get; set; }

        string Priority { get; set; }
    }
}