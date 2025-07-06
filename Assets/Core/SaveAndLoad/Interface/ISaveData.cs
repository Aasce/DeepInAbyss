namespace Asce.Managers.SaveLoads
{
    public interface ISaveData<T>
    {
        public void Save(in T target);
    }
}