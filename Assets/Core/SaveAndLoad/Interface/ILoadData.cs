namespace Asce.Managers.SaveLoads
{
    public interface ILoadData<T> where T : class
    {
        public bool Load(T data); 
    }
}