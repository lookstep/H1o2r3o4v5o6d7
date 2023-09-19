using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// ДолжностьОрганизации
    /// </summary>
    public class JobTitle1CUpp
    {
        /// <summary>
        /// Ид записи в 1с
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty("Description")]
        public string Name { get; set; }

        /// <summary>
        /// Код должности
        /// </summary>
        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}

