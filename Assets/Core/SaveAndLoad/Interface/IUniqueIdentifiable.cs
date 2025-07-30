namespace Asce.Managers.SaveLoads
{
    public interface IUniqueIdentifiable : IGameObject
    {
        public string ID { get; }
    }
}