using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Project.TaskParameters
{
    /// <summary>
    /// Source of the initial state of a task.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InitialStateSource
    {
        /// <summary>Values specified for model groups.</summary>
        Values,

        /// <summary>A result produced by a task of the same physics.</summary>
        Result
    }

    /// <summary>
    /// Initial state of a task.
    /// </summary>
    [Serializable]
    public sealed class InitialState
    {
        /// <summary>Source used to initialize the task.</summary>
        public InitialStateSource Source { get; set; } = InitialStateSource.Values;

        /// <summary>Physical fields grouped by names of groups from GroupData.</summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<InitialCondition> Conditions { get; set; } = new List<InitialCondition>();

        /// <summary>Result used when <see cref="Source"/> is <see cref="InitialStateSource.Result"/>.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ResultReference Result { get; set; }
    }
}
