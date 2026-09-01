using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Project.TaskParameters
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LinearSolverKind
    {
        BiCGStab,
        ConjugateGradient,
        SymmetricUtdu
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatrixStorageKind
    {
        GeneralCsr,
        SymmetricCsr
    }

    [Serializable]
    public class SolverSettings
    {
        [JsonConstructor]
        public SolverSettings(LinearSolverKind solver, MatrixStorageKind matrixStorage)
        {
            Solver = solver;
            MatrixStorage = matrixStorage;
        }

        public int MaxIter { get; set; }
        [JsonProperty(Required = Required.Always)]
        public LinearSolverKind Solver { get; }
        [JsonProperty(Required = Required.Always)]
        public MatrixStorageKind MatrixStorage { get; }

        public float MaxRelaxation { get; set; } = 1.5f;
        public float Relaxation { get; set; }

        public float Precision { get; set; }

        public string Priority { get; set; } = "Наивысший";
    }
}
