using System.Collections.Generic;

namespace Asce.Managers.SaveLoads
{
    public interface IGroupData<T>
    {
        List<T> Items { get; }
    }
}