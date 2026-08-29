

namespace Project.TaskParameters
{
    public interface IThermalDependent
    {
        ThermalFile ThermalFile { get; set; }

        double ThermalLoad { get; set; }

        bool ThermalLoadIsFile { get; set; }
    }
}
