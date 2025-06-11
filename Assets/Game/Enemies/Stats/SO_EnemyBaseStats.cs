using Asce.Game.Stats;
using UnityEngine;

namespace Asce.Game.Entities.Enemies
{
    [CreateAssetMenu(menuName = "Asce/Entities/Enemy Base Data", fileName = "Enemy Base Data")]
    public class SO_EnemyBaseStats : SO_CreatureBaseStats, IBaseStatsData
    {
        [SerializeField, Min(0f)] protected float _jumpForce;


        public float JumpForce => _jumpForce;
    }
}
