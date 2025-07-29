using Asce.Game.Enviroments.HiddenAreas;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class HiddenAreaData : SaveData, ISaveData<HiddenArea>, ILoadData<HiddenArea>
    {
        public string id;
        public bool isOpened;

        public void Save(in HiddenArea target)
        {
            this.id = target.ID;
            isOpened = target.IsOpened;
        }

        public bool Load(HiddenArea target)
        {
            if (target == null) return false;
            (target as IReceiveData<bool>).Receive(isOpened);
            return true;
        }
    }
}