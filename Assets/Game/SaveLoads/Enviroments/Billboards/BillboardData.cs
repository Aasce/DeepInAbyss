using Asce.Game.Enviroments;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class BillboardData : SaveData, ISaveData<Billboard>, ILoadData<Billboard>
    {
        public string id;
        public List<NoticeData> notices = new();

        public void Save(in Billboard target)
        {
            this.id = target.ID;
            this.notices.Clear();

            foreach (Notice notice in target.Notices)
            {
                if (notice == null) continue;

                NoticeData data = new();
                data.Save(notice);
                this.notices.Add(data);
            }
        }

        public bool Load(Billboard target)
        {
            if (target == null) return false;
            target.ClearNotices();

            foreach (var data in notices)
            {
                target.AddNotice(data.Create());
            }
            return true;
        }
    }
}