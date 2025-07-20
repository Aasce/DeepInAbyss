using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Combats
{
    public class AttackEventArgs : System.EventArgs
    {
        [SerializeField] private AttackType _attackType;


        public AttackType AttackType  => _attackType;


        public AttackEventArgs(AttackType attackType)
        {
            _attackType = attackType;
        }
    }
}