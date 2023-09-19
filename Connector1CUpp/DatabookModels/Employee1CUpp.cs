using Newtonsoft.Json;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// СотрудникиОрганизаций
    /// </summary>
    public class Employee1CUpp
    {
        /// <summary>
        /// Уникальный ИД в рамках базы 1с
        /// </summary>
        [JsonProperty("Ref_Key")]
        public string Id { get; set; }

        /// <summary>
        /// Табельный номер
        /// </summary>
        [JsonProperty("Code")]
        public string PersonnelNumber { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [JsonProperty("Description")]
        public string Name { get; set; }

        /// <summary>
        /// ИД ФизЛица из справочника ФизическиеЛица
        /// </summary>
        [JsonProperty("Физлицо_Key")]
        public string PersonId { get; set; }

        /// <summary>
        /// ИД подразделения из справочника ПодразделенияОрганизаций
        /// </summary>
        [JsonProperty("ТекущееПодразделениеОрганизации_Key")]
        public string DepartmentId { get; set; }

        /// <summary>
        /// ИД должности из справочника ДолжностиОрганизаций
        /// </summary>
        [JsonProperty("ТекущаяДолжностьОрганизации_Key")]
        public string JobTitleId { get; set; }

        /// <summary>
        /// Вид договора (своство перечисление)
        /// </summary>
        [JsonProperty("ВидДоговора")]
        public string ContractKind { get; set; }
    }
}

