using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterStats : CreatureStats, IHasOwner<Character>, IStatsController<SO_CharacterBaseStats>
    {
        [SerializeField] protected Stat _jumpForce = new();

        /// <summary>
        ///     Reference to the character that owns this stats controller.
        /// </summary>
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }
        public new SO_CharacterBaseStats BaseStats => base.BaseStats as SO_CharacterBaseStats;

        public Stat JumpForce => _jumpForce;


        public override void LoadBaseStats()
        {
            base.LoadBaseStats();

            JumpForce.AddAgent(gameObject, "base stats", BaseStats.JumpForce, StatValueType.Plat);
        }

        public override void UpdateStats(float deltaTime)
        {
            base.UpdateStats(deltaTime);

            JumpForce.Update(deltaTime);
        }
    }
}