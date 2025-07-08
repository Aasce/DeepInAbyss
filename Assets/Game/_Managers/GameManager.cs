using Asce.Game.FloatingTexts;
using Asce.Game.Players;
using Asce.Game.SaveLoads;
using Asce.Game.UIs;
using Asce.Game.VFXs;
using Asce.Managers;

namespace Asce.Game
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            _ = Player.Instance;
            _ = SaveLoadManager.Instance;
            _ = UIScreenCanvasManager.Instance;
            _ = StatValuePopupManager.Instance;
            _ = VFXsManager.Instance;
        }
    }
}
