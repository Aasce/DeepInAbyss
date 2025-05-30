using Asce.Game.UIs;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterUI : CreatureUI, IHasOwner<Character>, IWorldUI
    {
        [SerializeField] protected UIHealthBar _healthBar;

        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        public UIHealthBar HealthBar => _healthBar;

    }
}