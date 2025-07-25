using Asce.Game.Entities;
using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Equipments.Events
{
    public sealed class IronHelmetEquipEvent : EquipEvent
    {
        [SerializeField] private StatValue _armorValue = new(10f, StatValueType.Plat);
        [SerializeField] private StatValue _viewRadiusValue = new(5f, StatValueType.Plat);

        public string AddArmorReason => "Iron Helmet add armor";
        public string AddViewReason => "Iron Helmet add view";

        public StatValue ArmorValue => _armorValue;
        public StatValue ViewRadiusValue => _viewRadiusValue;

        public override void OnEquip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.AddAgent(creature.gameObject, AddArmorReason, _armorValue);
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.AddAgent(creature.gameObject, AddViewReason, _viewRadiusValue);
        }

        public override void OnUnequip(ICreature creature)
        {
            if (creature == null) return;
            if (creature.Stats == null) return;
			if (creature.Stats is IHasDefense hasDefense)
				hasDefense.DefenseGroup.Armor.RemoveAgent(creature.gameObject, AddArmorReason);
			
			if (creature.Stats is IHasViewRadius hasViewRadius)
				hasViewRadius.ViewRadius.RemoveAgent(creature.gameObject, AddViewReason);
        }
    }
}
