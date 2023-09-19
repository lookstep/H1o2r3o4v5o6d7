using DirectumConnector.DatabookModels;
using System;


namespace DirectumConnector.DocModels
{
    public class OrderBaseRx : DocumentBase
    {

        /// <summary>
        /// Номер
        /// </summary>
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime? DocumentDateNew { get; set; }

        /// <summary>
        /// Содержание
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Наименование (не имя, имя формируется автоматически)
        /// </summary>
        public string TitleName { get; set; }

        /// <summary>
        /// Подразделение
        /// </summary>
        public DepartmentRx Department { get; set; }

        /// <summary>
        /// Подготовил
        /// </summary>
        public EmployeeRx PreparedBy { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public EmployeeRx Assignee1C { get; set; }

        /// <summary>
        /// Ид записи в 1С: ДО
        /// </summary>
        public string Id1CDo { get; set; }

        /// <summary>
        /// Наша организация
        /// </summary>
        public BusinessUnitRx BusinessUnit { get; set; }

    }
}
