using Asce.Game.Entities;

namespace Asce.Game.Entities.Ores
{
    public class OreSpoils : EntitySpoils, IHasOwner<Ore>
    {
        public new Ore Owner
        {
            get => base.Owner as Ore;
            set => base.Owner = value;
        }
    }
}
