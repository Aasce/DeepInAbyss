using Asce.Game.UIs;
using Asce.Managers.SaveLoads;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class UIAllData : SaveData
    {
        public List<UIWindowData> windows = new();


        public UIAllData()
        {
            foreach (UIWindow window in UIScreenCanvasManager.Instance.WindowsController.Windows)
            {
                if (window == null) continue;
                UIWindowData windowData = new();
                windowData.Save(window);
                windows.Add(windowData);
            }
        }


        public void Load()
        {
            foreach (UIWindowData windowData in windows)
            {
                if (windowData == null) continue;
                UIWindow window = UIScreenCanvasManager.Instance.WindowsController.GetWindowByName(windowData.name);
                windowData.Load(window);
            }
        }
    }
}