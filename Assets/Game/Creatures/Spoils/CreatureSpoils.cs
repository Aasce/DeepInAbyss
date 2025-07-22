using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureSpoils : EntitySpoils, IHasOwner<Creature>
    {
        public new Creature Owner
        {
            get => base.Owner as Creature;
            set => base.Owner = value;
        }
    }
}