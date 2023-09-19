using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// ПодразделенияОрганизации
    /// </summary>
    public class Department1CUpp
    {
        /// <summary>
        /// ИД записи в 1с
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }


        /// <summary>
        /// ИД записи о вышестоящем подразделения из Catalog_ПодразделенияОрганизаций_Parent
        /// </summary>
        [JsonProperty("Parent_Key")]
        public string ParentDepartmentId { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [JsonProperty("Description")]
        public string Name { get; set; }

        /// <summary>
        /// Возможно краткое наименование
        /// </summary>
        [JsonProperty("ПрефиксКассовыхДокументов")]
        public string ShortName { get; set; }

        /// <summary>
        /// Код полразделения
        /// </summary>
        [JsonProperty("Code")]
        public string Code { get; set; }
    }
}

