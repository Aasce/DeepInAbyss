using Asce.Game.UIs;
using Asce.Game.UIs.Creatures;

namespace Asce.Game.Entities
{
    public interface IHasUI<T> : IEntity where T : IEntityUI
    {
        T UI { get; }
    }
}