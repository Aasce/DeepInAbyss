using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{

    public class CharacterAction : CreatureAction, IHasOwner<Character>, IMovable
    {
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }



    }
}
