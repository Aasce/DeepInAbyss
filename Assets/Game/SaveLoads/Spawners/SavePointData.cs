using Asce.Game.Spawners;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class SavePointData : ISaveData<ISavePoint>, ILoadData<ISavePoint>
    {
        public string id;
        public bool isActive;

        public void Save(in ISavePoint target)
        {
            this.id = target.ID;
            isActive = target.IsActive;
        }

        public bool Load(ISavePoint target)
        {
            if (target == null) return false;
            target.Receive(isActive);
            return true;
        }
    }
}