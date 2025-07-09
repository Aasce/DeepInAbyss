using System;
using UnityEngine;

namespace Asce.Game.Stats
{
    [Serializable]
    public class DefenseGroupStats : IGroupStats
    {
        [SerializeField] private Stat _armor = new(StatType.Armor);
        [SerializeField] private Stat _resistance = new(StatType.Resistance);
        [SerializeField] private ResourceStat _shield = new(StatType.Shield);

        public Stat Armor => _armor;
        public Stat Resistance => _resistance;
        public ResourceStat Shield => _shield;

        public DefenseGroupStats()
        {
            Shield.OnCurrentValueChanged += Shield_OnCurrentValueChanged;
        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Clear(bool isForceClear = false)
        {
            Armor.Clear(isForceClear);
            Resistance.Clear(isForceClear);
            Shield.Clear(isForceClear);
        }

        public virtual void Reset()
        {
            Armor.Reset();
            Resistance.Reset();
            Shield.Reset();
        }

        protected virtual void Shield_OnCurrentValueChanged(object sender, Managers.ValueChangedEventArgs args)
        {
            if (Shield.IsEmpty) Shield.ClearAgents();
        }
    }
}