using Asce.Game.UIs;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class UIWindowData : SaveData, ISaveData<UIWindow>, ILoadData<UIWindow>
    {
        public string name;
        public Vector2 position;
        public bool isShow;

        public void Save(in UIWindow target)
        {
            if (target == null) return;
            name = target.name;
            position = target.transform.localPosition;
            isShow = target.IsShow;
        }

        public bool Load(UIWindow target)
        {
            if (target == null) return false;
            target.transform.localPosition = position;
            target.SetVisible(isShow);
            return true;
        }

    }
}