namespace Asce.Game.Entities
{
    public interface IHasSpoils<T> where T : class
    {
        public T Spoils { get; }
    }
}