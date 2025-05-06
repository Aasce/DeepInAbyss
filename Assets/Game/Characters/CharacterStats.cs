using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterStats : CreatureStats, IStatsController<SO_CharacterBaseStats>
    {
        public new SO_CharacterBaseStats BaseStats => base.BaseStats as SO_CharacterBaseStats;


    }
}