namespace Asce.Game.Entities
{
    public interface IHasInteraction<T>
    {
        T Interaction { get; }
    }
}