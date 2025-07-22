using Asce.Game.UIs.Stats;
using Asce.Managers.Utils;
using TMPro;
using UnityEngine;

namespace Asce.Game.UIs.Ores
{
    public class UIOreCanvas : UIEntityCanvas, IWorldUI
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
