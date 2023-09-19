using Newtonsoft.Json;
using System;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// Справочник ФизическиеЛица
    /// </summary>
    public class Person1CUpp
    {
        /// <summary>
        /// Ид сущности в 1с
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        /// <summary>
        /// Полное ФИО
        /// </summary>
        [JsonProperty("Description")]
        public string FullName { get; set; }

        /// <summary>
        /// ДатаРождения
        /// </summary>
        [JsonProperty("ДатаРождения")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Актуальное имя из реестра ФИОФизЛиц
        /// </summary>
        [JsonIgnore]
        public string FirstName { get; set; }

        /// <summary>
        /// Актуальная фамилия из реестра ФИОФизЛиц
        /// </summary>
        [JsonIgnore]
        public string LastName { get; set; }

        /// <summary>
        /// Актуальное отчество из реестра ФИОФизЛиц
        /// </summary>
        [JsonIgnore]
        public string MiddleName { get; set; }
    }
}

