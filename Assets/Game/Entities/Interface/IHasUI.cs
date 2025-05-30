using Asce.Game.UIs;

namespace Asce.Game.Entities
{
    public interface IHasUI<T> where T : IWorldUI
    {
        T UI { get; }
    }
}