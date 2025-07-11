namespace Asce.Game.Entities
{
    public interface IHasStatusEffect<T>
    {
        T StatusEffect { get; }
    }
}