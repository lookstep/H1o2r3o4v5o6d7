using Newtonsoft.Json;
using System;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// Регистр ФИО ФизЛиц
    /// </summary>
    public class PersonNameRegister1CUpp
    {
        /// <summary>
        /// Дата начала дествия данных о физлице
        /// </summary>
        [JsonProperty("Period")]
        public DateTime Period { get; set; }

        /// <summary>
        /// Ид записи из справочника ФизическиеЛица
        /// </summary>
        [JsonProperty("ФизЛицо_Key")]
        public string Person1CUppId { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [JsonProperty("Фамилия")]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty("Имя")]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [JsonProperty("Отчество")]
        public string MiddleName { get; set; }

        /// <summary>
        /// ФизЛицо
        /// </summary>
        [JsonProperty("ФизЛицо")]
        public Person1CUpp Person { get; set; }
    }
}

