using Asce.Game.UIs.Stats;
using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.UIs.Creatures
{
    public class UICreatureCanvas : UIEntityCanvas, IWorldUI
    {
        [Header("Elements")]
        [SerializeField] protected UIHealthBar _healthBar;

        public UIHealthBar HealthBar => _healthBar;

        protected override void RefReset()
        {
            base.RefReset();
            this.LoadComponent(out _canvas);
        }
    }
}
