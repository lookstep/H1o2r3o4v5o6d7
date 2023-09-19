using System.Collections.Generic;

namespace Connector1CUpp.DatabookModels
{
    /// <summary>
    /// Результат ответа odata при запросе списка сущностей из 1С:УПП
    /// </summary>
    /// <typeparam name="T">Тип сущности 1С:УПП</typeparam>
    internal class ResultList1CUpp<T>
    {
        public string OdataMetadata { get; set; }

        public List<T> Value { get; set; }
    }
}
