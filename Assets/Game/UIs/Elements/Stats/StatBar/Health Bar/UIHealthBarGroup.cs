using Asce.Game.Stats;
using Asce.Managers.UIs;
using UnityEngine;
using UnityEngine.UI;

namespace Asce.Game.UIs.Stats
{
    public class UIHealthBarGroup : UIResourceStatBarGroup<UIHealthBar, TimeBasedResourceStat>
    {
        public virtual void SetStat(TimeBasedResourceStat health, ResourceStat shield)
        {
            this.SetStat(health);
            StatBar.SetShield(shield);
        }
    }
}