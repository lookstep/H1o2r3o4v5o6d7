using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// Организация
    /// </summary>
    public class BusinessUnit1CUpp
    {

        /// <summary>
        /// Ид сущности в 1с
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
         [JsonProperty("НаименованиеПолное")]
        public string FullName { get; set; }

        /// <summary>
        /// Краткое наименование
        /// </summary>
        [JsonProperty("Description")]
        public string ShortName { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [JsonProperty("Code")]
        public string Code { get; set; }

    }
}

