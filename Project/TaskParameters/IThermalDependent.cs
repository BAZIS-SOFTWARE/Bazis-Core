

namespace Project.TaskParameters
{
    public interface IThermalDependent
    {
        string ThermalFile { get; set; }

        double ThermalLoad { get; set; }

        bool ThermalLoadIsFile { get; set; }
    }
}
