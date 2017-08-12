using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IJsonDataConverter<T>
    {
        IList<T> GetList(string[] stringArray);
        IList<T> GetList(string responseString);
    }
}
