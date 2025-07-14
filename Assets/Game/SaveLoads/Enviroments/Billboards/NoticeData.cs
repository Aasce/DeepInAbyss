using Asce.Game.Enviroments;
using Asce.Managers.SaveLoads;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class NoticeData : SaveData, ISaveData<Notice>, ICreateData<Notice>
    {
        public string name = string.Empty;
        public string description = string.Empty;

        public void Save(in Notice target)
        {
            name = target.Name;
            description = target.Description;
        }

        public Notice Create()
        {
            return new Notice()
            {
                Name = name,
                Description = description
            };
        }
    }
}