namespace Asce.Game.Stats
{

    public interface IGroupStats
    {
        public void Update(float deltaTime);
        public void Clear(bool isForceClear = false);
        public void Reset();
    }
}