namespace Asce.Managers.SaveLoads
{
    public interface IReceiveData<T>
    {
        public void Receive(T data);
    }
}