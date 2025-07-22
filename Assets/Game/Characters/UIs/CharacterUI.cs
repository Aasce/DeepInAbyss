using Asce.Game.UIs.Creatures;
using UnityEngine;

namespace Asce.Game.Entities.Characters
{
    public class CharacterUI : CreatureUI, IHasOwner<Character>, IEntityUI
    {
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        protected override void Start()
        {
            base.Start();

        }

        public override void Register()
        {
            base.Register();

        }
    }
}