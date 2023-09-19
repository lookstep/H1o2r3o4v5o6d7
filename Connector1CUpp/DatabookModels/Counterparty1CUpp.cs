using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// Контрагент
    /// </summary>
    public class Counterparty1CUpp
    {
        /// <summary>
        /// Ид в справочнике 1С Catalog_Контрагенты
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        /// <summary>
        /// Код контрагента
        /// </summary>
        [JsonProperty("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Наименование (краткое)
        /// </summary>
        [JsonProperty("Description")]
        public string Name { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        [JsonProperty("ИНН")]
        public string Tin { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        [JsonProperty("КПП")]
        public string Trrc { get; set; }

        /// <summary>
        /// Польное наименование
        /// </summary>
        [JsonProperty("НаименованиеПолное")]
        public string FullName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        [JsonProperty("ОГРН")]
        public string Psrn { get; set; }
    }
}
