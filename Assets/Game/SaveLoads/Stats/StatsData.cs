using Asce.Game.Entities;
using Asce.Game.Stats;
using Asce.Managers.SaveLoads;
using UnityEngine;

namespace Asce.Game.SaveLoads
{
    [System.Serializable]
    public class StatsData : SaveData, ISaveData<EntityStats>, ILoadData<EntityStats>
    {
        public StatData health = new();
        public StatData healScale = new();

        public StatData stamina = new();
        public StatData hunger = new();
        public StatData thirst = new();
        public StatData breath = new();

        public StatData strength = new();
        public StatData armor = new();
        public StatData resistance = new();
        public StatData shield = new();

        public StatData speed = new();
        public StatData viewRadius = new();
        public StatData jumpForce = new();

        public void Save(in EntityStats target)
        {
            if (target == null) return;

            if (target is IHasStamina hasStamina) stamina.Save(hasStamina.Stamina);
            if (target is IHasJumpForce hasJumpForce) jumpForce.Save(hasJumpForce.JumpForce);
            if (target is IHasSurvivalStats hasSurvival)
            {
                health.Save(hasSurvival.HealthGroup.Health);
                healScale.Save(hasSurvival.HealthGroup.HealScale);
                hunger.Save(hasSurvival.SustenanceGroup.Hunger);
                thirst.Save(hasSurvival.SustenanceGroup.Thirst);
                breath.Save(hasSurvival.SustenanceGroup.Breath);
            }
            if (target is IHasCombatStats hasCombat)
            {
                strength.Save(hasCombat.Strength);
                armor.Save(hasCombat.DefenseGroup.Armor);
                resistance.Save(hasCombat.DefenseGroup.Resistance);
                shield.Save(hasCombat?.DefenseGroup.Shield);
            }
            if (target is IHasUtilitiesStats hasUtilities)
            {
                speed.Save(hasUtilities.Speed);
                viewRadius.Save(hasUtilities.ViewRadius);
            }
        }

        public bool Load(EntityStats target)
        {
            if (target == null) return false;

            if (target is IHasStamina hasStamina) stamina.Load(hasStamina.Stamina);
            if (target is IHasJumpForce hasJumpForce) jumpForce.Load(hasJumpForce.JumpForce);
            if (target is IHasSurvivalStats hasSurvival)
            {
                health.Load(hasSurvival.HealthGroup.Health);
                healScale.Load(hasSurvival.HealthGroup.HealScale);
                hunger.Load(hasSurvival.SustenanceGroup.Hunger);
                thirst.Load(hasSurvival.SustenanceGroup.Thirst);
                breath.Load(hasSurvival.SustenanceGroup.Breath);
            }
            if (target is IHasCombatStats hasCombat)
            {
                strength.Load(hasCombat.Strength);
                armor.Load(hasCombat.DefenseGroup.Armor);
                resistance.Load(hasCombat.DefenseGroup.Resistance);
                shield.Load(hasCombat?.DefenseGroup.Shield);
            }
            if (target is IHasUtilitiesStats hasUtilities)
            {
                speed.Load(hasUtilities.Speed);
                viewRadius.Load(hasUtilities.ViewRadius);
            }
            return true;
        }
    }
}