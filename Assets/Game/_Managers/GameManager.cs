using Asce.Managers;
using UnityEngine;

namespace Asce.Game
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            _ = Players.Player.Instance;
            _ = SaveLoads.SaveLoadManager.Instance;
            _ = Quests.QuestsManager.Instance;
            _ = Spawners.SavePointManager.Instance;
            _ = Spawners.EntitiesSpawnManager.Instance;
            _ = Items.ItemObjectsManager.Instance;
            _ = StatusEffects.StatusEffectsManager.Instance;
            _ = UIs.UIScreenCanvasManager.Instance;
            _ = FloatingTexts.StatValuePopupManager.Instance;
            _ = VFXs.VFXsManager.Instance;
        }
    }
}
