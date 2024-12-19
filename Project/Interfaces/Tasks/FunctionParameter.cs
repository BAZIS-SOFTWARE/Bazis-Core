namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// FunctionParameter
    /// </summary>
    public class FunctionParameter
    {
        public float Value { get; set; }

        public string Name { get; set; }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name} {Value}";
        }
    }
}