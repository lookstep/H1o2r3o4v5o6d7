using System.Collections.Generic;

namespace DirectumConnector
{
    /// <summary>
    /// Результат ответа odata при запросе списка сущностей из Rx
    /// </summary>
    /// <typeparam name="T">Тип сущности из Rx</typeparam>
    public class ResultListRx<T>
    {
        public string OdataContext { get; set; }

        public List<T> Value { get; set; }
    }
}

