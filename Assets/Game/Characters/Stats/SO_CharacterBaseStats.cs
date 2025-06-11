using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities
{
    [CreateAssetMenu(menuName = "Asce/Entities/ControlledCreature Base Data", fileName = "ControlledCreature Base Data")]
    public class SO_CharacterBaseStats : SO_CreatureBaseStats, IBaseStatsData
    {
        [SerializeField, Min(0f)] protected float _jumpForce;


        public float JumpForce => _jumpForce;
    }
}
