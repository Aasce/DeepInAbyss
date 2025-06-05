using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterStats : CreatureStats, IHasOwner<Character>, IStatsController<SO_CharacterBaseStats>
    {
        [SerializeField] protected JumpForceStat _jumpForce = new();

        /// <summary>
        ///     Reference to the characterView that owns this stats controller.
        /// </summary>
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }
        public new SO_CharacterBaseStats BaseStats => base.BaseStats as SO_CharacterBaseStats;

        public JumpForceStat JumpForce => _jumpForce;


        public override void LoadBaseStats()
        {
            base.LoadBaseStats();

            JumpForce.AddAgent(gameObject, baseStatsReason, BaseStats.JumpForce, StatValueType.Base).ToNotClearable();
        }

        public override void UpdateStats(float deltaTime)
        {
            base.UpdateStats(deltaTime);

            JumpForce.Update(deltaTime);
        }

        public override void ClearStats(bool isForceClear = false)
        {
            base.ClearStats(isForceClear);
            JumpForce.Clear(isForceClear);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            JumpForce.Reset();
        }
    }
}