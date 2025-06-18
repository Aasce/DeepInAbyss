using Asce.Game.Entities;
using UnityEngine;

namespace Asce.Game.Combats
{
    public class AttackEventArgs : System.EventArgs
    {
        [SerializeField] private ICreature _attacker;
        [SerializeField] private AttackType _attackType;


        public ICreature Attacker => _attacker;
        public AttackType AttackType  => _attackType;


        public AttackEventArgs(ICreature attacker, AttackType attackType)
        {
            _attacker = attacker;
            _attackType = attackType;
        }
    }
}